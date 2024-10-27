using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Areas.Admin.Repository;
using ShoppingDemo.Models;
using ShoppingDemo.Models.ViewModels;
using ShoppingDemo.Repository;
using System.Security.Claims;

namespace ShoppingDemo.Controllers
{
	public class AccountController : Controller
	{
		private AppDbContext _context;
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
		private readonly IEmailSender _emailSender;

		public AccountController(IEmailSender emailSender, AppDbContext appDbContext, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager)
		{
			_emailSender = emailSender;
			_userManager = userManager;
			_signInManager = signInManager;
			_context = appDbContext;
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
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}
		public async Task<IActionResult> NewPass(AppUserModel user, string token)
		{
			var checkuser = await _userManager.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();

			if (checkuser != null)
			{
				ViewBag.Email = checkuser.Email;
				ViewBag.Token = token;
			}
			else
			{
				TempData["error"] = "Email not found or token is not right";
				return RedirectToAction("ForgotPass", "Account");
			}

			return View();
		}

		public async Task<IActionResult> ForgetPass(string returnUrl)
		{
			return View();
		}
		public async Task<IActionResult> UpdateAccount()
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				// User is not logged in, redirect to login
				return RedirectToAction("Login", "Account"); // Replace "Account" with your controller
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
			if (user == null)
			{
				return NotFound();
			}
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> UpdateInfoAccount(AppUserModel user)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userbyId = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (userbyId == null)
            {
                return NotFound();
            }
            else
            {
               
                _context.Update(userbyId);
                await _context.SaveChangesAsync();
                TempData["success"] = "Update Account Success";
            }
            return RedirectToAction("UpdateAccount", "Account");
        }


        public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
		{
			var checkuser = await _userManager.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();

			if (checkuser != null)
			{
				// update user with new password and token
				string newtoken = Guid.NewGuid().ToString();
				// Hash the new password
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);

				checkuser.PasswordHash = passwordHash;
				checkuser.Token = newtoken;

				await _userManager.UpdateAsync(checkuser);
				TempData["success"] = "Password updated successfully.";
				return RedirectToAction("Login", "Account");
			}
			else
			{
				TempData["error"] = "Email not found or token is not right";
				return RedirectToAction("ForgotPass", "Account");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
		{
			var checkMail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

			if (checkMail == null)
			{
				TempData["error"] = "Email not found";
				return RedirectToAction("ForgotPass", "Account");
			}
			else
			{
				string token = Guid.NewGuid().ToString();
				// update token to user
				checkMail.Token = token;
				_context.Update(checkMail);
				await _context.SaveChangesAsync();
				var receiver = checkMail.Email;
				var subject = "Change password for user " + checkMail.Email;
				var message = "Click on link to change password " +
					"<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email="
					+ checkMail.Email + "&token=" + token + "'>";
				await _emailSender.SenEmailAsync(receiver, subject, message);
			}
			TempData["success"] = "An email has been sent to your registered email address with password reset instructions.";
			return RedirectToAction("ForgetPass", "Account");

		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult
			result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
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
		public async Task<IActionResult> History()
		{
			if (!User.Identity?.IsAuthenticated ?? true)
			{
				// User is not logged in, redirect to login
				return RedirectToAction("Login", "Account"); // Replace "Account" with your controller
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var Orders = await _context.Orders
				.Where(od => od.UserName == userEmail)
				.OrderByDescending(od => od.Id)
				.ToListAsync();
			ViewBag.UserEmail = userEmail;
			return View(Orders);

		}
		public async Task<IActionResult> CancelOrder(string ordercode)
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				// User is not logged in, redirect to login
				return RedirectToAction("Login", "Account");
			}
			try
			{
				var order = await _context.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
				order.Status = 3;
				_context.Update(order);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return BadRequest("An error occurred while canceling the order.");
			}
			return RedirectToAction("History", "Account");
		}

		[HttpPost]
		public async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
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
