using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Controllers
{
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;
        public BrandController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            BrandModel brand = _context.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
            if (brand == null)
                return RedirectToAction("Index");
            var productsBybrand = _context.Products.Where(p => p.BrandId == brand.Id);
            return View(await productsBybrand.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}
