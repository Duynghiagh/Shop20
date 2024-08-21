using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace ShoppingDemo.Repository.Componnents
{
	public class CategoriesViewComponent: ViewComponent
	{
		private readonly AppDbContext _context;
        public CategoriesViewComponent(AppDbContext appDbContext)
        {   
            _context = appDbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
            =>View(await _context.Categories.ToListAsync());
        
    }
}
