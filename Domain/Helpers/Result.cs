namespace Domain.Helpers
{
    public class Result
    {
        public Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class Result<T> : Result
    {
        public Result(bool success, string message, T data) : base(success, message)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
