using BookSearchWeb.Custom.Filters;
using BookSearchWeb.Interfaces;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookSearchWeb.Custom.Analyzers
{
    public class MyCustomAnalyzer : Analyzer
    {
        private LuceneVersion _version;
        private ISynonymEngine _engine;
        public MyCustomAnalyzer(LuceneVersion version, ISynonymEngine engine)
        {
            _version = version;
            _engine = engine;
        }
        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            StandardTokenizer src = new StandardTokenizer(_version, reader);
            TokenStream result = new StandardFilter(_version, src);
            result = new LowerCaseFilter(_version, result);
            result = new StopFilter(_version, result, StandardAnalyzer.STOP_WORDS_SET);
            result = new SynonymFilter(result, _engine);
            result = new PorterStemFilter(result);
            TokenStreamComponents T = new TokenStreamComponents(src, result);

            return T;
        }
    }
}
