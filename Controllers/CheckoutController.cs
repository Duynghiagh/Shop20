using Microsoft.AspNetCore.Mvc;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;
using System.Security.Claims;

namespace ShoppingDemo.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly AppDbContext _context;
		public CheckoutController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}
		public async Task<IActionResult> Checkout()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (userEmail == null)
			{
				return RedirectToAction("Login", "Account");
			}
			else
			{
				var orderCode = Guid.NewGuid().ToString();
				var orderItem = new OrderModel();
				orderItem.OrderCode = orderCode;
				orderItem.UserName = userEmail;
				orderItem.Status = 1;
				orderItem.CreatedDate = DateTime.Now;
				_context.Add(orderItem);
				_context.SaveChanges();
				List<CartItemModel> cartItems = HttpContext.Session.GetJson
			   <List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
				foreach (var cart in cartItems)
				{
					var orderdetails = new OrderDetails();
					orderdetails.UserName = userEmail;
					orderdetails.OrderCode = orderCode;
					orderdetails.ProductId = cart.ProductId;
					orderdetails.Price = cart.Price;
					orderdetails.Quantity = cart.Quantity;
					_context.Add(orderdetails);
					_context.SaveChanges();

				}
				HttpContext.Session.Remove("Cart");
				TempData["success"] = "Đơn hàng đã được tạo, vui long chờ duyệt đơn hàng";
				return RedirectToAction("Index", "Cart");

			}
			return View();
		}
	}
}
