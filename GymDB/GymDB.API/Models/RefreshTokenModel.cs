namespace GymDB.API.Models
{
    public class RefreshTokenModel
    {
        public RefreshTokenModel(string refreshToken, DateTime refreshTokenCreated, DateTime refreshTokenExpires)
        {
            RefreshToken = refreshToken;
            RefreshTokenCreated = refreshTokenCreated;
            RefreshTokenExpires = refreshTokenExpires;
        }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenCreated { get; set; }

        public DateTime RefreshTokenExpires { get; set; }
    }
}
