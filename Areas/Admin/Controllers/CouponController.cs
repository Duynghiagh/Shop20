using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly AppDbContext _context;
        public CouponController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var coupon_list = await _context.Counpons.ToListAsync();
            ViewBag.Counpons = coupon_list;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CounponModel counponModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(counponModel);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm mã giảm giá thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model đang có 1 vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }
    }
}
