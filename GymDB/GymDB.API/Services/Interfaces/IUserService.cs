﻿using GymDB.API.Data.Entities;
using GymDB.API.Models;
using GymDB.API.Models.User;

namespace GymDB.API.Services.Interfaces
{
    public interface IUserService
    {
        bool AreAnyUsersRegistered();

        List<User> GetAll();

        User? GetById(Guid id);

        User? GetByEmail(string email);

        User? GetByEmailAndPassword(string email, string password);

        bool IsUserAlreadyRegisteredWithEmail(string email);

        string GetHashedPassword(string password);

        void Add(User user);

        void Update(User user);
    }
}
