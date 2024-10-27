using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<IActionResult> Index(string slug = "", string sort_by = "", string startprice = "", string endprice = "")
        {
            CategoryModel category = _context.Categories.Where(c => c.Slug == slug).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Slug = slug;
            IQueryable<ProductModel> productsByCategory = _context.Products.Where(p => p.CategoryId == category.Id);
            var count = await productsByCategory.CountAsync();
            if (count > 0)
            {
                if (sort_by == "price_increase")
                {
                    productsByCategory = productsByCategory.OrderBy(p => p.Price);
                }
                else if (sort_by == "price_decrease")
                {
                    productsByCategory = productsByCategory.OrderByDescending(p => p.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                }
                else if (sort_by == "price_oldest")
                {
                    productsByCategory = productsByCategory.OrderBy(p => p.Id);
                }
                //Loc gia sp
                else if (startprice != "" && endprice != "")
                {
                    decimal startPriceValue;
                    decimal endPriceValue;

                    if (decimal.TryParse(startprice, out startPriceValue) && decimal.TryParse(endprice, out endPriceValue))
                    {
                        productsByCategory = productsByCategory.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                    }
                    else
                    {
                        productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                    }
                }
                else
                {
                    productsByCategory = productsByCategory.OrderByDescending(p => p.Id);

                }

            }
            return View(await productsByCategory.ToListAsync());
        }

    }
}
