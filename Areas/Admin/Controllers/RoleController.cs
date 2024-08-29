using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _rollManager;
        private readonly AppDbContext _context;

        public RoleController(AppDbContext appDbContext, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> rollManager)
        {
            _userManager = userManager;
            _rollManager = rollManager;
            _context = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.OrderByDescending(p => p.Id).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var role = await _rollManager.FindByIdAsync(Id);
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(string Id,IdentityRole model)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var role = await _rollManager.FindByIdAsync(Id);
                if (role == null)
                {
                    return NotFound();
                }
                role.Name = model.Name;
                try
                {
                    await _rollManager.UpdateAsync(role);
                    TempData["success"] = "Đã sửa Role thành công!";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi khi sửa Role");
                }
            }
            return View(model ?? new IdentityRole { Id = Id});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            if(!_rollManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _rollManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return Redirect("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var role = await _rollManager.FindByIdAsync(Id);
            if (role == null)
            {
                return NotFound();
            }
            try
            {
                await _rollManager.DeleteAsync(role);
                TempData["success"] = "Đã xóa Role thành công!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi xóa Role");
            }
            return RedirectToAction("Index");
        }
    }
}
