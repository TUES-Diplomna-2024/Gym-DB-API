using System.Net;

namespace GymDB.API.Exceptions
{
    public class OkException : HttpException
    {
        public OkException(string message) : base(HttpStatusCode.OK, message) { }
    }
}
