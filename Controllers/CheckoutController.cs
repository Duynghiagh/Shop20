using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShoppingDemo.Areas.Admin.Repository;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;
using System.Security.Claims;

namespace ShoppingDemo.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IEmailSender _emailSender;
		public CheckoutController(IEmailSender emailSender, AppDbContext appDbContext)
		{
			_context = appDbContext;
			_emailSender = emailSender;
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
                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                decimal shippingPrice = 0;
                //nhan coupon for cookie
                var coupon_code = Request.Cookies["CouponTitle"];

                if (shippingPriceCookie != null)
                {
                    var shippingPriceJson = shippingPriceCookie;
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                }
				orderItem.ShippingCost = shippingPrice;
				orderItem.CouponCode = coupon_code;

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
					var product = await _context.Products.Where(p=>p.Id == cart.ProductId).FirstAsync();
					product.Quantity -= cart.Quantity;
					product.Sold += cart.Quantity;
					_context.Update(product);
					_context.Add(orderdetails);
					_context.SaveChanges();

				}
				HttpContext.Session.Remove("Cart");
				//Send mail order when success
				var receiver = userEmail;
				var subject = "Đặt hàng thành công";
				var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé";
				await _emailSender.SenEmailAsync(receiver, subject, message);
				TempData["success"] = "Đơn hàng đã được tạo, vui long chờ duyệt đơn hàng";
				return RedirectToAction("History", "Account");

			}
			return View();
		}
	}
}
