using BookSearchWeb.Custom.Analyzers;
using BookSearchWeb.Custom.Similarity;
using BookSearchWeb.Interfaces;
using BookSearchWeb.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookSearchWeb.Services
{
    public class BookService : IBookService
    {
        const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        readonly Regex regex = new Regex(@"([+-]?)([\w\d_/]+)", RegexOptions.Compiled);
        readonly Regex regex1 = new Regex(@"([+-]?)([\w\d_/]+):([\w\d_]+)", RegexOptions.Compiled);
        private Analyzer standardanalyzer;
        private Analyzer mycustomanalyzer;
        DirectoryReader reader;
        String indexPath;

        private Book filter1(string v)
        {
            string[] s = v.Split(',');
            return new Book(s[2].ToLower(), s[1].ToLower());
        }
        public BookService( ISynonymEngine synonymEngine)
        {
            standardanalyzer = new StandardAnalyzer(AppLuceneVersion);
            mycustomanalyzer = new MyCustomAnalyzer(AppLuceneVersion, synonymEngine);
             List<Book> books = File.ReadAllLines(@"./Data/books.csv").Skip(1)
                                                       .SkipLast(11000)
                                                       .Select(v => filter1(v)).ToList();
            var basePath = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData);
            indexPath = Path.Combine(basePath, "index");
            if (System.IO.Directory.Exists(indexPath))
            {
                System.IO.Directory.Delete(indexPath,true);
            }
            using FSDirectory dir = FSDirectory.Open(indexPath);
            
            IDictionary<string, Analyzer> analyzerPerField = new Dictionary<string, Analyzer>();
            analyzerPerField["author"] = standardanalyzer;
            analyzerPerField["title"] = mycustomanalyzer;
            PerFieldAnalyzerWrapper aWrapper =
              new PerFieldAnalyzerWrapper(standardanalyzer, analyzerPerField);
            var analyzer = aWrapper;
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using (IndexWriter writer = new IndexWriter(dir, indexConfig))
            {
                foreach (Book book in books)
                {
                    var doc = new Document{
                                        new TextField("author",
                                        book.author,
                                        Field.Store.YES),
                                        new TextField("title",
                                        book.title,
                                        Field.Store.YES)
                                    };

                    writer.AddDocument(doc);
                    writer.Flush(triggerMerge: false, applyAllDeletes: false);
                }
            
            reader = writer.GetReader(applyAllDeletes: false);

            
            }

        }

        private Occur GetOccur(string oq)
        {
            switch (oq)
            {
                case "+":
                    return Occur.MUST;

                case "-":
                    return Occur.MUST_NOT;

                default:
                    return Occur.SHOULD;

            }
        }
        private Query Create_Query(string query)
        {
            BooleanQuery boolQuery = new BooleanQuery();
            bool flag = false;
            MatchCollection match1 = regex1.Matches(query);
            foreach( Match match in match1)
            {
                flag = true;
                string field = match.Groups[3].Value;
                Analyzer an = field == "title" ? mycustomanalyzer : standardanalyzer;
                boolQuery.Add(new QueryParser(AppLuceneVersion, field, an).Parse(match.Groups[2].Value),
                                    GetOccur(match.Groups[1].Value));
                query = query.Replace(match.Value, "");
            }

            MatchCollection match0 = regex.Matches(query);
            foreach (Match match in match0)
            {

                flag = true;
                boolQuery.Add(new MultiFieldQueryParser(AppLuceneVersion, new string[] { "author", "title" }, mycustomanalyzer).Parse(match.Groups[2].Value), GetOccur(match.Groups[1].Value));
            }
            if (!flag)
            {
                return null;
            }
            return boolQuery;
        }
        public List<Book> SearchBooks(string query)
        {
            var searcher = new IndexSearcher(reader);
            searcher.Similarity = new CustomSimilarity();
            Query query_author_title = Create_Query(query);
            if (query_author_title == null)
                return null;
            var hits = searcher.Search(query_author_title, 10).ScoreDocs;
            List<Book> b = new List<Book>();
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                if (hit.Score > 0)
                {

                    Book bb = new Book(foundDoc.Get("author"), foundDoc.Get("title"), hit.Score);
                    if (!b.Contains(bb))
                        b.Add(bb);
                    
                }

            }
            return b;
        }
        private Query Create_PrefixQuery(string query)
        {
            BooleanQuery boolQuery = new BooleanQuery();
            bool flag = false;
                MatchCollection match1 = regex.Matches(query);
            foreach(Match match in match1)
            {
                flag = true;
                BooleanQuery boolQuery_begin_author = boolQuery;
                boolQuery_begin_author.Add(new PrefixQuery(new Term("author", match.Groups[2].Value)), Occur.SHOULD);

                BooleanQuery boolQuery_begin_title = boolQuery;
                boolQuery.Add(new PrefixQuery(new Term("title", match.Groups[2].Value)), Occur.SHOULD);

                boolQuery = new BooleanQuery();
                boolQuery.Add(boolQuery_begin_title, Occur.SHOULD);
                boolQuery.Add(boolQuery_begin_author, Occur.SHOULD);
            }
         

            if (!flag)
            {
                return null;
            }
            return boolQuery;
        }
        public List<BookResult> predictSearch(string query)
        {
            var searcher = new IndexSearcher(reader);
            searcher.Similarity = new CustomSimilarity();
            Query query_author_title = Create_PrefixQuery(query);
            if (query_author_title == null)
                return null;
            var hits = searcher.Search(query_author_title, 4).ScoreDocs;
            List<BookResult> b = new List<BookResult>();
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                if (hit.Score > 0)
                {
                    string author = foundDoc.Get("author");
                    if(author.Contains(query))
                    {
                        BookResult bb = new BookResult(author , "author");
                        if (!b.Contains(bb))
                            b.Add(bb);
                    }
                }

            }
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                if (hit.Score > 0)
                {
                    string title = foundDoc.Get("title");
                    if (title.Contains(query))
                    {
                        BookResult bb = new BookResult(title, "title");
                        if (!b.Contains(bb))
                            b.Add(bb);
                    }
                }
            }
            
            return b;
        }
    }
}
