using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TelerikMvcApp1.Data;
using Category = TelerikMvcApp1.Models.Category;
using Product = TelerikMvcApp1.Models.Product;
using Supplier = TelerikMvcApp1.Models.Supplier;

namespace TelerikMvcApp1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Data.NorthwindEntities _context = new Data.NorthwindEntities();

        public ActionResult Index()
        {
            ViewData["defaultSupplier"] = _context.Suppliers.Select(
                s => new Supplier { Id = s.SupplierID, Name = s.CompanyName }).First();
            ViewData["defaultCategory"] = _context.Categories.Select(
                c => new Category { Id = c.CategoryID, Name = c.CategoryName }).First();

            return View();
        }

        public JsonResult Get([DataSourceRequest] DataSourceRequest request)
        {
            var products = _context.Products.Select(p => new Product
            {
                Id = p.ProductID,
                Name = p.ProductName,
                UnitsInStock = (short)p.UnitsInStock,
                UnitPrice = p.UnitPrice,
                Discontinued = p.Discontinued,
                Supplier = new Supplier
                {
                    Id = p.Supplier.SupplierID,
                    Name = p.Supplier.CompanyName
                },
                Category = new Category
                {
                    Id = p.Category.CategoryID,
                    Name = p.Category.CategoryName
                }
            }).OrderByDescending(p=>p.Id);
            return Json(products.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Update([DataSourceRequest] DataSourceRequest request, Product product)
        {
            if (product != null && ModelState.IsValid)
            {
                var updatedProduct = _context.Products.FirstOrDefault(p => p.ProductID == product.Id);
                if (updatedProduct != null)
                {
                    updatedProduct.ProductName = product.Name;
                    updatedProduct.UnitPrice = product.UnitPrice;
                    updatedProduct.UnitsInStock = product.UnitsInStock;
                    updatedProduct.Discontinued = product.Discontinued;
                    updatedProduct.SupplierID = product.Supplier.Id;
                    updatedProduct.CategoryID = product.Category.Id;

                    _context.Entry(updatedProduct).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create([DataSourceRequest] DataSourceRequest request, Product product)
        {
            if (product != null && ModelState.IsValid)
            {
                var newProduct = new Data.Product
                {
                    ProductName = product.Name,
                    UnitPrice = product.UnitPrice,
                    UnitsInStock = product.UnitsInStock,
                    Discontinued = product.Discontinued,
                    SupplierID = product.Supplier.Id,
                    CategoryID = product.Category.Id
                };

                _context.Products.Add(newProduct);
                _context.SaveChanges();
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public  JsonResult Delete([DataSourceRequest] DataSourceRequest request, Product product)
        {
            if (product != null)
            {
                var deletedProduct = _context.Products.First(p => p.ProductID == product.Id);

                try
                {
                    _context.Products.Remove(deletedProduct);
                    _context.SaveChanges();
                }
                catch (Exception exception)
                {
                    return this.Json(new DataSourceResult
                    {
                        Errors = string.Format("Product deleting was faild. Check if you trying to delete product that already using in users orders. Original server error: \n {0}",exception.Message)
                    });
                }
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }
    }
}