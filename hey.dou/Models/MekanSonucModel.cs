namespace hey.dou.Models
{
	// Frontend'e gidecek olan sadeleştirilmiş veri paketi.
	// Koordinatlar kaldırıldı, sadece kat ve resim bilgisi var.
	public class MekanSonucModel
	{
		public string MekanKodu { get; set; }     // Örn: D-144
		public string Aciklama { get; set; }      // Örn: Yazılım Lab.
		public string KatAdi { get; set; }        // Örn: 1. Kat
		public string KrokiResimAdi { get; set; } // Örn: kat_D1.png
	}
}