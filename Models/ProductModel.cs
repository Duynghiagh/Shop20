using ShoppingDemo.Repository.Valication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingDemo.Models
{
   
    public class ProductModel
	{
		[Key]
		public long Id { get; set; }
		[Required( ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
		public string Name { get; set; }
		public string Slug { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên mô tả sản phẩm")]
		public string Description { get; set; }
		[Required( ErrorMessage = "Yêu cầu nhập giá sản phẩm")]
			[Column(TypeName = "decimal(20, 2)")]
			public decimal Price { get; set; }
		[Required,Range(1,int.MaxValue,ErrorMessage =("Chọn một thương hiệu"))]
		public int BrandId { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = ("Chọn một danh mục"))]

        public int CategoryId { get; set; }
		public CategoryModel Category { get; set; }
		public BrandModel Brand { get; set; }
		public string Image { get; set; }
		[NotMapped]
		[FileExtenstion]

		public IFormFile? ImageUpload { get; set; }


	}
}
