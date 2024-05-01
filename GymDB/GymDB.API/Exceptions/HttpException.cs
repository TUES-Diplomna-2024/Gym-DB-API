using System.Net;

namespace GymDB.API.Exceptions
{
    public class HttpException : Exception
    {
        public HttpException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
