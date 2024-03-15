using GymDB.API.Data.ValidationAttributes;

namespace GymDB.API.Models.User
{
    public class UserDeleteModel
    {
        [Password]
        public string Password { get; set; }
    }
}
