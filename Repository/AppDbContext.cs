﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingDemo.Models;

namespace ShoppingDemo.Repository
{
	public class AppDbContext:IdentityDbContext<AppUserModel>
	{
        public AppDbContext()
        {
            
        }

		public AppDbContext(DbContextOptions options) : base(options)
		{

		}
		public DbSet<BrandModel> Brands { get; set; }
		public DbSet<ProductModel> Products { get; set; }
		public DbSet<CategoryModel> Categories { get; set; }
		public DbSet<OrderModel> Orders { get; set; }
		public DbSet<OrderDetails> OrderDetails { get; set; }
		public DbSet<SliderModel> Sliders { get; set; }
		public DbSet<WishlistModel> wishlistModels{ get; set; }
		public DbSet<CompareModel> compareModels{ get; set; }
		public DbSet<ProductQuantityModel> productQuantities{ get; set; }
		public DbSet<RatingModel> ratingModels{ get; set; }
		public DbSet<ShippingModel> Shippings {get; set; }
		public DbSet<CounponModel> Counpons {get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=NGHIAFIN\\MSSQLSERVER1;Database=ShopCart;Trusted_Connection=True;TrustServerCertificate=True\r\n");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
