using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;
using System.Diagnostics;

namespace ShoppingDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUserModel> _userManager;


        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _context = appDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Include("Category").Include("Brand").ToList();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddWishList(long Id)
        {
            var user = await _userManager.GetUserAsync(User);
            var wishListProduct = new WishlistModel
            {
                ProductId = Id,
                UserId = user.Id
            };

            _context.wishlistModels.Add(wishListProduct);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm vào yêu thích thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Có lỗi khi thêm vao yêu thích");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddCompare(long Id)
        {
            var user = await _userManager.GetUserAsync(User);
            var CompareProduct = new CompareModel
            {
                ProductId = Id,
                UserId = user.Id
            };

            _context.compareModels.Add(CompareProduct);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm vào so sánh thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Có lỗi khi thêm vào so sánh");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        public async Task<IActionResult> Compare()
        {
            var compare_p = await (from c in _context.compareModels
                                   join p in _context.Products on c.ProductId equals p.Id
                                   join u in _context.Users on c.UserId equals u.Id
                                   select new { User = u, Product = p, Compares = c }).ToListAsync();
            return View(compare_p);
        }
        public async Task<IActionResult> DeleteCompare(int Id)
        {
            CompareModel compare = await _context.compareModels.FindAsync(Id);
            _context.compareModels.Remove(compare);
            await _context.SaveChangesAsync();
            TempData["success"] = "So sánh đã xóa thành công";
            return RedirectToAction("Compare","Home");
        }
        public async Task<IActionResult> DeleteWishlist(int Id)
        {
            WishlistModel wishlist= await _context.wishlistModels.FindAsync(Id);
            _context.wishlistModels.Remove(wishlist);
            await _context.SaveChangesAsync();
            TempData["success"] = "Yêu thích đã xóa thành công";
            return RedirectToAction("Wishlist", "Home");
        }
        public async Task<IActionResult> Wishlist()
        {
            var wishlist_p = await (from w in _context.wishlistModels
                                   join p in _context.Products on w.ProductId equals p.Id
                                   join u in _context.Users on w.UserId equals u.Id
                                   select new { User = u, Product = p, Wishlists = w }).ToListAsync();
            return View(wishlist_p);
        }
    }
}

