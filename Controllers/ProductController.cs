using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Controllers
{
	public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public IActionResult Index()
		{
			return View();
		}
        public async Task<IActionResult> Search(string searchTerm)
		{
			
            var products = await _context.Products
      .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
      .ToListAsync();
            ViewBag.Keyword = searchTerm;
            return View(products);
        }
		public async Task<IActionResult> Details(long Id)
		{
			if(Id == null) return RedirectToAction("Index");
            var productsById = _context.Products.Where(p => p.Id == Id).FirstOrDefault();
			var relateProducts = await _context.Products
				.Where(p=>p.CategoryId == productsById.CategoryId 
				&& p.Id != productsById.Id).Take(4).ToListAsync();
			ViewBag.RelatedProducts = relateProducts;
            return View(productsById);
		}

	}
}
