using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingDemo.Models;
using ShoppingDemo.Models.ViewModels;

namespace ShoppingDemo.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
      
      
        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

        public async Task<ActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var item = new CreateAccountViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                UserName = user.UserName
            };

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostProfile(CreateAccountViewModel req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            user.FullName = req.FullName;
            user.PhoneNumber = req.Phone;
            var rs = await _userManager.UpdateAsync(user);
            if (rs.Succeeded)
            {
                return RedirectToAction("Profile");
            }
            return View(req);
        }
        public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl});
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult 
			result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password,false,false);
				if (result.Succeeded)
				{
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Tài khoản hoặc mật khẩu đang bị sai xin vui lòng nhập lại");
			}
			return View(loginVM);
		}



		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser,user.Password);
				if (result.Succeeded)
				{
					TempData["Success"] = "Tạo user thành công ";
					return Redirect("/account/login");
				}
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}

		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}

	}
}
