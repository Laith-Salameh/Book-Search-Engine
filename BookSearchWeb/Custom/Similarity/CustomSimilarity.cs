using Lucene.Net.Search.Similarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSearchWeb.Custom.Similarity
{
    public class CustomSimilarity : DefaultSimilarity
    {
        public override float Tf(float freq)
        {
            float tf = (float)Math.Sqrt(freq);
            if (freq < 2)
            {
                return tf / 2;
            }


            return tf;
        }

        public override float Idf(long docFreq, long numDocs)
        {
            float idf = (float)Math.Log(numDocs / (docFreq + 1)) + 1;
            if (numDocs < 3)
            {
                return idf * 2;
            }
            return idf;

        }

    }
}
