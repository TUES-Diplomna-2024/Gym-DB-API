using GymDB.API.Data.Enums;

namespace GymDB.API.Models.User
{
    public class UserProfileExtendedModel : UserProfileModel
    {
        public AssignableRole? AssignableRole { get; set; }

        public bool IsRemovable { get; set; }
    }
}
