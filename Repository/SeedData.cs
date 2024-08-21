using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;
using System;

namespace ShoppingDemo.Repository
{
	public class SeedData
	{
		public static void SeedingData(AppDbContext _context)
		{
			_context.Database.Migrate();
			if (!_context.Products.Any())
			{
				CategoryModel Macbook = new CategoryModel { Name = "Apple", Slug = "apple", Description = "Apple is Large brand in the world", Status = 1 };
				CategoryModel pc = new CategoryModel { Name = "pc", Slug = "pc", Description = "pc is Large brand in the world", Status = 1 };
				BrandModel Apple = new BrandModel { Name = "Apple", Slug = "apple", Description = "Apple is Large brand in the world", Status = 1 };
				BrandModel Sony = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "Samsung is Large brand in the world", Status = 1 };
				_context.Products.AddRange(

					new ProductModel { Name = "Macbook", Slug = "Macbook", Description = "Macbook is best", Image = "1.jpg", Category = Macbook, Brand = Apple, Price = 10223 },
				new ProductModel { Name = "pc", Slug = "pc", Description = "pc is best", Image = "2.jpg", Category = pc, Brand = Sony, Price = 1413 }
				);
				_context.SaveChanges();
			}
		}
	}
}
