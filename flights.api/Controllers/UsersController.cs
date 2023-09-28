using flights.application.DTO;
using flights.application.Interfaces;
using flights.crosscutting.Messages.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace flights.api.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsersController : MainController
    {
        private readonly IUserService _userService;
        private readonly INotificator _notificator;

        public UsersController(IUserService userService,
            INotificator notification) : base(notification)
        {
            _userService = userService;
            _notificator = notification;
        }

        /// <summary>
        /// Autenticação na API
        /// </summary>
        [HttpPost]
        [Route("users/auth")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Authenticate([FromBody]LoginDTO login)
        {
            object result = new object();
            try
            {
                result = new { token = _userService.Authenticate(login) };
            }
            catch (Exception e)
            {
                _notificator.notify(e.Message);
            }
            return CustomResponse(result);
        }
    }
}