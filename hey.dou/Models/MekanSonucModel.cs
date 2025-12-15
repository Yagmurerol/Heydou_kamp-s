namespace hey.dou.Models
{
    public class MekanSonucModel
    {
        // = null!; ekleyerek hatayı susturduk
        public string MekanKodu { get; set; } = null!;
        public string? Aciklama { get; set; }
        public string KatAdi { get; set; } = null!;
        public string KrokiResimAdi { get; set; } = null!;
    }
}