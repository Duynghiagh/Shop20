using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrderModel> category = _context.Orders.ToList(); //33 datas

            const int pageSize = 10; //10 items/trang

            if (pg < 1) //page < 1;
            {
                pg = 1; //page ==1
            }
            int recsCount = category.Count(); //33 items;

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = category.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var DetailsOrder = await _context.OrderDetails.Include
         (od => od.Product).Where(od => od.OrderCode == ordercode).ToListAsync();
            var Order= _context.Orders.Where(O => O.OrderCode == ordercode).First();
            ViewBag.ShippingCost = Order.ShippingCost;
            ViewBag.Status = Order.Status;
            return View(DetailsOrder);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại" });
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = status;

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật đơn hàng: " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(string ordercode)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);

            try
            {
                await _context.SaveChangesAsync();
                // Chuyển hướng về trang Index của ViewOrder
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the order");
            }
        }


    }
}
