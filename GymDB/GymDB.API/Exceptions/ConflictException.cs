using System.Net;

namespace GymDB.API.Exceptions
{
    public class ConflictException : HttpException
    {
        public ConflictException(string message) : base(HttpStatusCode.Conflict, message) { }
    }
}
