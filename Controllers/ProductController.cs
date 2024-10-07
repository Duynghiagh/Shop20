using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Models.ViewModels;
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
            if (Id == null) return RedirectToAction("Index");
            var productsById = _context.Products.Include(p => p.Rating).Where(p => p.Id == Id).FirstOrDefault();
            var relateProducts = await _context.Products
                .Where(p => p.CategoryId == productsById.CategoryId
                && p.Id != productsById.Id).Take(4).ToListAsync();
            ViewBag.RelatedProducts = relateProducts;
            var viewModel = new ProductDetailsViewModel
            {
                ProductDetails = productsById,
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentProduct(RatingModel rating)
        {
            if (ModelState.IsValid)
            {
                var ratingEntity = new RatingModel
                {
                    ProductId = rating.ProductId,
                    Name = rating.Name,
                    Email = rating.Email,
                    Comment = rating.Comment,
                    Star = rating.Star
                };
                _context.ratingModels.Add(ratingEntity);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm đánh giá thành công";
                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Thêm đánh giá thất bại";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return RedirectToAction("Details", new { id = rating.ProductId });
            }
            return Redirect(Request.Headers["Referer"]);

        }

    }
}
