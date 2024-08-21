using Microsoft.AspNetCore.Mvc;

namespace ShoppingDemo.Controllers
{
    public class LoginController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
