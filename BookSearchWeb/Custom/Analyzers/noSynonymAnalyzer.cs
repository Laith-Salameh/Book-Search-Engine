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
    public class noSynonymAnalyzer : Analyzer
    {
        private LuceneVersion _version;
        public noSynonymAnalyzer(LuceneVersion version)
        {
            _version = version;
        }
        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            StandardTokenizer src = new StandardTokenizer(_version, reader);
            TokenStream result = new StandardFilter(_version, src);
            result = new LowerCaseFilter(_version, result);
            result = new PorterStemFilter(result);
            TokenStreamComponents T = new TokenStreamComponents(src, result);

            return T;
        }
    }
}
