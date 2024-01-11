﻿using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using GymDB.API.Data.Settings;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationSettings settings;

        private readonly IUserService userService;
        private readonly IRoleService roleService;
        private readonly IJwtService jwtService;
        private readonly ISessionService sessionService;

        public UserController(IConfiguration config, IUserService userService, IRoleService roleService, IJwtService jwtService, ISessionService sessionService)
        {
            settings = new ApplicationSettings(config);

            this.userService = userService;
            this.roleService = roleService;
            this.jwtService = jwtService;
            this.sessionService = sessionService;
        }

        [HttpPost("signup")]
        public IActionResult SignUp(UserSignUpModel signUpAttempt)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            if (userService.IsUserAlreadyRegisteredWithEmail(signUpAttempt.Email))
                return Conflict();

            User user = new User(signUpAttempt);

            if (!roleService.AssignUserRole(user, settings.DBSeed.DefaultRole))
                return StatusCode(500, $"Role '{settings.DBSeed.DefaultRole}' could not be found!");

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
            Session? userSession = sessionService.GetSessionByRefreshToken(signOutAttempt.RefreshToken);

            if (userSession == null)
                return Unauthorized();

            sessionService.Remove(userSession);

            return Ok();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(UserSessionRetrievalModel refreshAttempt)
        {
            Session? userSession = sessionService.GetSessionByRefreshToken(refreshAttempt.RefreshToken);

            if (userSession == null)
                return Unauthorized("You must sign in again!");

            return Ok(jwtService.GenerateNewJwtToken(userSession.User));
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult GetUserById(Guid id)
        {
            User? user = userService.GetById(id);

            if (user == null)
                return NotFound($"User with id {id} could not be found!");
            
            return Ok(new UserProfileModel(user));
        }

        [HttpGet, Authorize(Roles = "ADMIN")]
        public IActionResult GetAllUsers()
        {
            List<UserProfileModel> users = userService.GetAll()
                                                      .Select(user => new UserProfileModel(user))
                                                      .ToList();

            return Ok(users);
        }
    }
}
