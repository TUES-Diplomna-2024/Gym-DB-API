namespace GymDB.API.Models.User
{
    public class UserAuthModel
    {
        public UserAuthModel(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
