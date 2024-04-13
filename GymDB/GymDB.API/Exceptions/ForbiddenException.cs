using System.Net;

namespace GymDB.API.Exceptions
{
    public class ForbiddenException : HttpException
    {
        public ForbiddenException(string message) : base(HttpStatusCode.Forbidden, message) { }
    }
}
