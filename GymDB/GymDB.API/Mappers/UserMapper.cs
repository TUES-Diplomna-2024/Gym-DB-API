using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Data.Settings;
using GymDB.API.Models.User;

namespace GymDB.API.Mappers
{
    public static class UserMapper
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

        public static User ToEntity(this RootAdmin rootAdmin)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                OnModified = DateTime.UtcNow,

                Username = rootAdmin.Username,
                Email = rootAdmin.Email,
                Password = rootAdmin.Password,
                BirthDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Gender = Gender.Other,
                Height = 80,
                Weight = 80
            };
        }

        public static void ApplyUpdateModel(this User user, UserUpdateModel update)
        {
            user.Username = update.Username;
            user.BirthDate = update.BirthDate;
            user.Gender = update.Gender;
            user.Height = update.Height;
            user.Weight = update.Weight;
        }

        public static void SetRole(this User user, Role role)
        {
            user.RoleId = role.Id;
            user.Role = role;
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
