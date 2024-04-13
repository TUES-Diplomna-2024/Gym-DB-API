using System.Net;

namespace GymDB.API.Exceptions
{
    public class HttpException : Exception
    {
        public HttpException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = (int)statusCode;
        }

        public int StatusCode { get; private set; }
    }
}
