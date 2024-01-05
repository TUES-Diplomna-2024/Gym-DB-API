namespace GymDB.API.Models
{
    public class RefreshAttemptModel
    {
        public Guid UserId { get; set; }

        public string RefreshToken { get; set; }
    }
}
