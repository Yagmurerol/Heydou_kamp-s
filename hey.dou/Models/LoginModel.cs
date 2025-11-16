using Microsoft.EntityFrameworkCore;

namespace hey.dou.Models
{
	public class LoginModel
	{
		public string Email { get; set; }
		public string Sifre { get; set; }
		public string Rol { get; set; }
	}
}

