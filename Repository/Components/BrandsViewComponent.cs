using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShoppingDemo.Repository.Componnents
{
	public class BrandsViewComponent: ViewComponent
	{
		private readonly AppDbContext _context;
        public BrandsViewComponent(AppDbContext appDbContext)
        {   
            _context = appDbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
            =>View(await _context.Brands.ToListAsync());
    }
}
