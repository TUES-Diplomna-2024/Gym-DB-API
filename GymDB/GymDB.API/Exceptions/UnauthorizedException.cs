using System.Net;

namespace GymDB.API.Exceptions
{
    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }
}
