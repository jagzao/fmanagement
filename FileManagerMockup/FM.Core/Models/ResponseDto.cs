namespace FM.Core.Models
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public Exception exception { get; set; }
    }
}
