namespace hey.dou.Models
{
	// Frontend'e göndereceğimiz "Kroki Paketi"
	public class MekanSonucModel
	{
		public string MekanKodu { get; set; }     // Örn: D-144
		public string Aciklama { get; set; }      // Örn: Yazılım Lab-1
		public string KatAdi { get; set; }        // Örn: 1. Kat
		public string KrokiResimAdi { get; set; } // Örn: kat_1_kroki.png

		// Pinin konumu (Resmin sol üst köşesinden % uzaklık)
		public int PinX { get; set; }
		public int PinY { get; set; }
	}
}
