using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSearchWeb.Models
{
    public class Book
    {
        public string author { get; set; }
        public string title { get; set; }
        public float score = 0;

        public string tostring()
        {
            return author + ',' + title + ',' + score;
        }
        public Book(string a, string t)
        {
            author = a;
            title = t;
        }
        public Book(string a, string t, float x)
        {
            author = a;
            title = t;
            score = x;
        }

        public override bool Equals(object b)
        {
            if (b == null) return false;
            Book nb = b as Book;
            if (nb.author.ToLower() == this.author.ToLower() && nb.title.ToLower() == this.title.ToLower())
                return true;
            else
                return false;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(author, title);
        }
        
    }
}
