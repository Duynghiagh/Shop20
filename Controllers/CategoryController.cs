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
        public async Task<IActionResult> Index(string Slug = "")
        {
            CategoryModel category = _context.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
            if (category == null)
                return RedirectToAction("Index");
            var productsByCategory = _context.Products.Where(p => p.CategoryId == category.Id);
            return View(await productsByCategory.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}
