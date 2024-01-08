namespace GymDB.API.Models.User
{
    public class UserAuthModel
    {
        public UserAuthModel(string jwt, string refreshToken)
        {
            Jwt = jwt;
            RefreshToken = refreshToken;
        }

        public string Jwt { get; set; }

        public string RefreshToken { get; set; }
    }
}
