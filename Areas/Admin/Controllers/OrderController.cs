using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _context.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var DetailsOrder = await _context.OrderDetails.Include
         (od => od.Product).Where(od => od.OrderCode == ordercode).ToListAsync();
            return View(DetailsOrder);
        }
    }
}
