namespace hey.dou.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(bool success, string message, T data = default, int statusCode = 200)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(bool success, string message, int statusCode = 200)
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;
        }
    }
}
