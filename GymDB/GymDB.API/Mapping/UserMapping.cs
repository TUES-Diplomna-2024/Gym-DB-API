using GymDB.API.Data.Entities;
using GymDB.API.Data.Settings.DBSeedModels;
using GymDB.API.Data.ValidationAttributes;
using GymDB.API.Models.User;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymDB.API.Mapping
{
    public static class UserMapping
    {
        public static User ToEntity(this UserSignUpModel signUpModel)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                OnModified = DateTime.UtcNow,

                Username = signUpModel.Username,
                Email = signUpModel.Email,
                Password = signUpModel.Password,
                BirthDate = signUpModel.BirthDate,
                Gender = signUpModel.Gender,
                Height = signUpModel.Height,
                Weight = signUpModel.Weight
            };
        }

        public static User ToEntity(this RootAdmin rootAdmin, Role role)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                OnModified = DateTime.UtcNow,
                RoleId = role.Id,
                Role = role,

                Username = rootAdmin.Username,
                Email = rootAdmin.Email,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(rootAdmin.Password, 13),
                BirthDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Gender = "other",
                Height = 60,
                Weight = 60
            };
        }

        public static UserProfileModel ToProfileModel(this User user)
        {
            return new UserProfileModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleName = user.Role.Name,
                RoleColor = user.Role.Color,
                Gender = user.Gender,
                Height = user.Height,
                Weight = user.Weight,
                BirthDate = user.BirthDate,
                OnCreated = user.OnCreated
            };
        }

        public static UserPreviewModel ToPreviewModel(this User user)
        {
            return new UserPreviewModel
            {
                Id = user.Id,
                Username = user.Username,
                RoleName = user.Role.Name,
                RoleColor = user.Role.Color,
                OnCreated = user.OnCreated,
                Age = CalculateAge(user.BirthDate)
            };
        }

        private static int CalculateAge(DateOnly birthDate)
        {
            DateOnly currDate = DateOnly.FromDateTime(DateTime.UtcNow);

            int age = currDate.Year - birthDate.Year;

            if (currDate.DayOfYear < birthDate.DayOfYear)
                age--;

            return age;
        }
    }
}
