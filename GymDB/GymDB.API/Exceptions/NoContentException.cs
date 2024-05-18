using System.Net;

namespace GymDB.API.Exceptions
{
    public class NoContentException : HttpException
    {
        public NoContentException() : base(HttpStatusCode.NoContent, "") { }
    }
}
