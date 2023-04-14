using BookSearchWeb.Interfaces;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSearchWeb.Custom.Filters
{
    public sealed class SynonymFilter : TokenFilter
    {
        private readonly ICharTermAttribute termAtt;
        private Stack<string> currentSynonyms;
        private PositionIncrementAttribute posAtt;
        private State currentState;
        private ISynonymEngine synonymEngine;
        public SynonymFilter(TokenStream input, ISynonymEngine syn_engine)
            : base(input)
        {
            termAtt = AddAttribute<ICharTermAttribute>();
            posAtt = (PositionIncrementAttribute)AddAttribute<IPositionIncrementAttribute>();

            currentSynonyms = new Stack<string>();
            synonymEngine = syn_engine;

        }


        public override bool IncrementToken()
        {

            if (currentSynonyms.Count > 0)
            {
                string synonym = currentSynonyms.Pop();
                RestoreState(currentState);
                termAtt.SetEmpty();
                termAtt.Append(synonym.ToCharArray(), 0, synonym.Length);
                posAtt.PositionIncrement = 0;
                return true;
            }
            if (!m_input.IncrementToken()) return false;

            string currentTerm = termAtt.Subsequence(0, termAtt.Length).ToString().ToLower();
            if (currentTerm != null)
            {
                var synonyms = DetermineSynonym(currentTerm);
                if (synonyms.Count == 1) return true;
                foreach (var synonym in synonyms)
                {
                    currentSynonyms.Push(synonym.ToLower());
                }
            }
            currentState = CaptureState();
            return true;
        }

        public List<string> DetermineSynonym(string word)
        {
            return synonymEngine.GetSynonyms(word);
        }
    }


}
