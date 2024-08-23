using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Add(long Id)
        {
            ProductModel product = await _context.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson
                <List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItems == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItems.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Add Item to cart Successfully";  
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
            List<CartItemModel> cart = HttpContext.Session.GetJson
               <List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
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
			TempData["success"] = "Increase Item quantity to cart Successfully";
			return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(long Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson
               <List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
           cart.RemoveAll(p=>p.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart",cart);
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
