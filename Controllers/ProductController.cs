using Microsoft.AspNetCore.Mvc;
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
		public async Task<IActionResult> Details(int Id)
		{

			if(Id == null) return RedirectToAction("Index");
            var productsById = _context.Products.Where(p => p.Id == Id).FirstOrDefault();
            return View(productsById);
		}
	}
}
