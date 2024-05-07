using System.Net;

namespace GymDB.API.Exceptions
{
    public class InternalServerErrorException : HttpException
    {
        public InternalServerErrorException(string message) : base(HttpStatusCode.InternalServerError, message) { }
    }
}
