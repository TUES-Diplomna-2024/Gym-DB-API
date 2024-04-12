using System.Net;

namespace GymDB.API.Models.Other
{
    public class ServiceResultModel
    {
        public ServiceResultModel(dynamic result, HttpStatusCode statusCode, string message = "")
        {
            Result = result;
            StatusCode = statusCode;
            Message = message;
        }

        public dynamic Result { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public string Message { get; private set; }
    }
}
