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
    }
}
