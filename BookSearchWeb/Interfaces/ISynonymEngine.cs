using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSearchWeb.Interfaces
{
    public interface ISynonymEngine
    {
        public Dictionary<string, List<string>> SynonymsDictionary { get; set; }
        public List<string> GetSynonyms(string s);
    }
}
