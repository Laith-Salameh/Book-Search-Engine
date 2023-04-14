using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSearchWeb.Models
{
    public class BookResult
    {
        public string label { get; set; }
        public string category { get; set; }
        

        public BookResult(string a, string t)
        {
            label = a;
            category = t;
        }
       
        public override bool Equals(object b)
        {
            if (b == null) return false;
            BookResult nb = b as BookResult;
            if (nb.label.ToLower() == this.label.ToLower() && nb.category.ToLower() == this.category.ToLower())
                return true;
            else
                return false;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(label, category);
        }
        
    }
}
