using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelerikMvcApp1.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly Data.NorthwindEntities _context = new Data.NorthwindEntities();

        // GET: Categories
        public JsonResult Index()
        {
            var categories = _context.Categories.Select(c => new Models.Category
            {
                Id = c.CategoryID,
                Name = c.CategoryName
            });

            return Json(categories, JsonRequestBehavior.AllowGet);
        }
    }
}