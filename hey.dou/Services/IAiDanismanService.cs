namespace hey.dou.Services
{
    public interface IAiDanismanService
    {
        Task<string> GetAnswerAsync(string question);
    }
}
