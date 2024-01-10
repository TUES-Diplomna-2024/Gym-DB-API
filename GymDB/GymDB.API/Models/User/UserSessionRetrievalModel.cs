namespace GymDB.API.Models.User
{
    public class UserSessionRetrievalModel
    {
        public Guid UserId { get; set; }

        public string RefreshToken { get; set; }
    }
}
