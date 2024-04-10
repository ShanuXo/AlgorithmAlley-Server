using System.Net;

namespace AlgorithmAlley.Models
{
    public class APIResponses<T>
    {
        public APIResponses()
        {
            ErrorMessage = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessage { get; set; }
        public object Result { get; set; }
    }
}
