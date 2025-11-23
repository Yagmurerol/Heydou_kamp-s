namespace hey.dou.Models
{
    public class AiDanismanViewModel
    {
        public string Question { get; set; } = string.Empty;   // null olmasın diye default
        public string? Answer { get; set; }
        public string? Error { get; set; }
    }
}
