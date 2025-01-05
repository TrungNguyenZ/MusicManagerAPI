namespace MusicManager.Models.Base
{
    public class ResponseData<T>: ResponseBase
    {
        public T data { get; set; }
    }
    public class ResponseBase
    {
        public bool isSuccess { get; set; } = true;
        public int code { get; set; } = 200;
        public string message { get; set; } = "Thành công!";
        public DateTime requestTime { get; set; } = DateTime.Now;
    }
}
