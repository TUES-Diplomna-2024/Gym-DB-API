using GymDB.API.Data.Entities;
using GymDB.API.Data.Settings.HelperClasses;

namespace GymDB.API.Mappers
{
    public static class RoleMapper
    {
        public static Role ToEntity(this RoleDefinition roleDef)
        {
            return new Role
            {
                Id = Guid.NewGuid(),
                Name = roleDef.Name,
                NormalizedName = roleDef.NormalizedName,
                Color = roleDef.Color
            };
        }
    }
}
