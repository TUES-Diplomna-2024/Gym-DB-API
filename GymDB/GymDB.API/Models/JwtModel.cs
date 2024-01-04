namespace GymDB.API.Models
{
    public class JwtModel
    {
        public JwtModel(string jwt)
        {
            Jwt = jwt;
        }

        public string Jwt { get; set; }
    }
}
