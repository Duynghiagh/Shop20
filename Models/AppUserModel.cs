using Microsoft.AspNetCore.Identity;

namespace ShoppingDemo.Models
{
	public class AppUserModel:IdentityUser
	{
		public string Ocupation{ get; set; }
		public string RoleId { get; set; }
	}
}
