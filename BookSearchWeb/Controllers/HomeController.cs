using BookSearchWeb.Interfaces;
using BookSearchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BookSearchWeb.Controllers
{
    public class HomeController : Controller
    {
        private IBookService _BookService;


        public HomeController(IBookService BookService)
        {
            _BookService = BookService;

        }

        public IActionResult Index()
        {

            return View();
        }

        [Route("book/search")]
        [Produces("application/json")]
        [HttpGet("search")]
        public IActionResult Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var searchChoices = new List<BookResult>();
                if (!string.IsNullOrEmpty(term))
                {
                    searchChoices = _BookService.predictSearch(term);
                    string res = string.Empty;
                    res = JsonConvert.SerializeObject(searchChoices, Formatting.Indented, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                }
                return Ok(searchChoices);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("book/getbooks")]
        [Produces("application/json")]
        public JsonResult GetBooks(string searchValue)
        {
            try
            {
                if (searchValue != "" && searchValue != null)
                {
                    var data = _BookService.SearchBooks(searchValue).ToList();

                    string res = string.Empty;
                    res = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    return Json(res);
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch
            {
                return Json(string.Empty);
            }
                
           
        }
    }
}
