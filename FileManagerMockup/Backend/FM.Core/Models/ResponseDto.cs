using System.Net;

namespace FM.Core.Models
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public HttpStatusCode Status { get; set; }
        public Exception Exception { get; set; }
    }
}
