using System.ComponentModel.DataAnnotations;

namespace ShoppingDemo.Models
{
	public class UserModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage ="Cần nhập Tên")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Cần nhập Email"),EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password),Required(ErrorMessage ="Cần nhập mật khẩu")]
		public string Password { get; set; }

	}
}
