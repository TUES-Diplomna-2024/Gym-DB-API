using System.Net;

namespace GymDB.API.Exceptions
{
    public class NoContentException : HttpException
    {
        public NoContentException(string message) : base(HttpStatusCode.NoContent, message) { }
    }
}
