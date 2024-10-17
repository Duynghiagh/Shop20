using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShippingController : Controller
    {
        private readonly AppDbContext _context;
        public ShippingController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var shippingList =await _context.Shippings.ToListAsync();
            ViewBag.Shippings = shippingList;
            return View();

        }
        [HttpPost]
        [Route("StoreShipping")]
        public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string phuong, string quan,string tinh, decimal price)
        {
            shippingModel.City = tinh;
            shippingModel.District= quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;
            try
            {
                var exitsingShipping = await _context.Shippings.AnyAsync(x=>x.City == tinh && x.District == quan && x.Ward == phuong);
                if (exitsingShipping)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp" });
                }
                _context.Shippings.Add(shippingModel);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm shipping thành công" });
            }
            catch (Exception)
            {

               return StatusCode(500,"Lỗi");
            }


        }
        public async Task<IActionResult> Delete(int Id)
        {
            ShippingModel shipping = await _context.Shippings.FindAsync(Id);
            _context.Shippings.Remove(shipping);
            await _context.SaveChangesAsync();
            TempData["success"] = "ship đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
