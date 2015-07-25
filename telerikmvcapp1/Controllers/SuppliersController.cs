using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelerikMvcApp1.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly Data.NorthwindEntities _context = new Data.NorthwindEntities();

        // GET: Supplier
        public JsonResult Index()
        {
            var suppliers = _context.Suppliers.Select(s => new Models.Supplier
            {
                Id = s.SupplierID,
                Name = s.CompanyName
            });

            return Json(suppliers, JsonRequestBehavior.AllowGet);
        }
    }
}