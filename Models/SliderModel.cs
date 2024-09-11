using ShoppingDemo.Repository.Valication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingDemo.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên Slider")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên mô tả ")]
        public string Description { get; set; }

        public int? Status { get; set; }
        public string Image { get; set; }
        [NotMapped]
        [FileExtenstion]

        public IFormFile? ImageUpload { get; set; }

    }
}
