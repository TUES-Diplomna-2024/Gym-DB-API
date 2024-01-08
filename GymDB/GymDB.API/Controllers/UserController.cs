﻿using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using GymDB.API.Models;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IJwtService jwtService;
        private readonly ISessionService sessionService;

        public UserController(IUserService userService, IJwtService jwtService, ISessionService sessionService)
        {
            this.userService = userService;
            this.jwtService = jwtService;
            this.sessionService = sessionService;
        }

        [HttpGet, Authorize]
        public IActionResult GetAllUsers()
        {
            List<UserProfileModel> users = userService.GetAll()
                                                      .Select(user => new UserProfileModel(user))
                                                      .ToList();

            return Ok(users);
        }

        [HttpPost("signup")]
        public IActionResult SignUp(UserSignUpModel signUpAttempt)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            if (userService.IsUserAlreadyRegisteredWithEmail(signUpAttempt.Email))
                return Conflict();

            User user = new User(signUpAttempt);
            userService.Add(user);

            string refreshToken = sessionService.CreateNewSession(user);

            return Ok(new UserAuthModel(jwtService.GenerateNewJwtToken(user), refreshToken));
        }

        [HttpPost("signin")]
        public IActionResult SignIn(UserSignInModel signInAttempt)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            User? user = userService.GetByEmailAndPassword(signInAttempt.Email, signInAttempt.Password);

            if (user == null)
                return Unauthorized();

            string refreshToken = sessionService.CreateNewSession(user);

            return Ok(new UserAuthModel(jwtService.GenerateNewJwtToken(user), refreshToken));
        }

        [HttpPost("signout")]
        public IActionResult SignOut(UserSessionRetrievalModel signOutAttempt)
        {
            Session? userSession = sessionService.GetUserSessionByRefreshToken(signOutAttempt.UserId, signOutAttempt.RefreshToken);

            if (userSession == null)
                return Unauthorized();

            sessionService.Remove(userSession);

            return Ok();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(UserSessionRetrievalModel refreshAttempt)
        {
            User? user = userService.GetById(refreshAttempt.UserId);

            if (user == null)
                return NotFound($"User with id {refreshAttempt.UserId} could not be found!");

            Session? userSession = sessionService.GetUserSessionByRefreshToken(refreshAttempt.UserId, refreshAttempt.RefreshToken);

            if (userSession == null)
                return Unauthorized("You must sign in again!");

            return Ok(jwtService.GenerateNewJwtToken(user));
        }
    }
}
