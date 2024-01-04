namespace GymDB.API.Models
{
    public class AuthModel
    {
        public AuthModel(string jwt, string refreshToken)
        {
            Jwt = jwt;
            RefreshToken = refreshToken;
        }

        public string Jwt { get; set; }

        public string RefreshToken { get; set; }
    }
}
