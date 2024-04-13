using System.Net;

namespace GymDB.API.Exceptions
{
    public class NotFoundException : HttpException
    {
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
    }
}
