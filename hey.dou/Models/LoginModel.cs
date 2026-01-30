using System.ComponentModel.DataAnnotations;

namespace hey.dou.Models
{
	public class LoginModel
	{
		[Required(ErrorMessage = "E-posta gereklidir")]
		[EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Şifre gereklidir")]
		[MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı")]
		public string Sifre { get; set; }

		[Required(ErrorMessage = "Rol seçiniz")]
		public string Rol { get; set; }
	}
}

