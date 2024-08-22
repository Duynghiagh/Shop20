using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ShoppingDemo.Models.ViewModels
{
	public class LoginViewModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Cần nhập Tên")]
		public string Username { get; set; }
	
		[DataType(DataType.Password), Required(ErrorMessage = "Cần nhập mật khẩu")]
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
