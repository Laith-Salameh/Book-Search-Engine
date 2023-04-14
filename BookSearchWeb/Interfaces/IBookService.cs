using BookSearchWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSearchWeb.Interfaces
{
    public interface IBookService
    {
        public List<Book> SearchBooks(string Query);
        public List<BookResult> predictSearch(string query);
    }
}
