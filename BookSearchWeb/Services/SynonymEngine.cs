using BookSearchWeb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSearchWeb.Services
{
    public class SynonymEngine : ISynonymEngine
    {
        public Dictionary<string, List<string>> SynonymsDictionary { get; set; }

        public SynonymEngine()
        {
            SynonymsDictionary = new Dictionary<string, List<string>>();

            List<string> s = new List<string>() { "harry", "laith", "thommas" };
            makeDic(s);
            s = new List<string>() { "small", "tiny", "micro" };
            makeDic(s);
            s = new List<string>() { "america", "usa", "syria" };
            makeDic(s);

        }
        private void makeDic(List<string> synonymgroup)
        {
            foreach (string g in synonymgroup)
            {
                if (!SynonymsDictionary.ContainsKey(g))
                    SynonymsDictionary.Add(g, synonymgroup);
                else
                    SynonymsDictionary[g] = SynonymsDictionary[g].Concat(synonymgroup).ToList();

            }
        }

        public List<string> GetSynonyms(string s)
        {
            if (SynonymsDictionary.ContainsKey(s))
                return SynonymsDictionary[s];
            else
                return new List<string>() { s };
        }
    }
}
