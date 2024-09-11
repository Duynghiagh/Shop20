using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using ShoppingDemo.Repository;

namespace ShoppingDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _context = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.OrderByDescending(p => p.Id).ToListAsync());
        }
   
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderModel slider)
        {
            if (ModelState.IsValid)
            {
                if (slider.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    slider.Image = imageName;

                }
                _context.Add(slider);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm slider thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model đang có 1 vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(slider);
        }
    }
}
