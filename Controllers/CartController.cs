﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Models.ViewModels;
using ShoppingDemo.Repository;
using System.Collections.Generic;

namespace ShoppingDemo.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson
                <List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),

            };
            return View(cartVM);
        }
        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }
        
        public async Task<IActionResult> Add(long Id, int quantity)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu
            ProductModel product = await _context.Products.FindAsync(Id);

            if (product == null)
            {
                return NotFound(); // Xử lý trường hợp sản phẩm không được tìm thấy
            }

            // Lấy giỏ hàng từ session hoặc khởi tạo giỏ hàng mới nếu chưa có
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tìm mục trong giỏ hàng tương ứng với sản phẩm đã cho
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id);

            if (cartItem == null)
            {
                // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới với số lượng được chỉ định
                cart.Add(new CartItemModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    Image = product.Image // Giả sử bạn có thuộc tính Image
                });
            }
            else
            {
                // Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng của nó
                cartItem.Quantity += 1;
            }

            // Lưu giỏ hàng đã cập nhật trở lại session
            HttpContext.Session.SetJson("Cart", cart);

            // Tùy chọn: Sử dụng TempData để hiển thị thông báo thành công trong yêu cầu tiếp theo
            TempData["success"] = "Sản phẩm đã được thêm vào giỏ hàng thành công";

            // Chuyển hướng về trang trước đó
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(Dictionary<long, int> cartItems)
        {
            // Lấy giỏ hàng từ session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Cập nhật số lượng cho từng sản phẩm trong giỏ hàng
            foreach (var item in cartItems)
            {
                var cartItem = cart.FirstOrDefault(c => c.ProductId == item.Key);
                if (cartItem != null)
                {
                    cartItem.Quantity = Math.Max(item.Value, 1);
                }
            }

            // Lưu giỏ hàng đã cập nhật trở lại session
            HttpContext.Session.SetJson("Cart", cart);

            // Thông báo thành công
            TempData["success"] = "Số lượng giỏ hàng đã được cập nhật thành công";

            // Chuyển hướng về trang trước đó
            return Redirect(Request.Headers["Referer"].ToString());
        }


        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson
               <List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(c => c.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Decrease Item quantity to cart Successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int Id)
        {
            ProductModel product = await _context.Products.Where(p => p.Id == Id).FirstOrDefaultAsync();
            List<CartItemModel> cart = HttpContext.Session.GetJson
               <List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity >= 1 && product.Quantity > cartItem.Quantity)
            {
                ++cartItem.Quantity;
                TempData["success"] = "Tăng số lượng sản phầm thành công";

            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["success"] = "Số lượng đã đặt đến giới hạn";


            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Increase Item quantity to cart Successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(long Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson
               <List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            cart.RemoveAll(p => p.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Remove Item of cart Successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear all Item of cart Successfully";

            return RedirectToAction("Index");
        }

    }
}
