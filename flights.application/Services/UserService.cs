using flights.application.DTO;
using flights.application.Interfaces;
using flights.crosscutting.Messages.Interfaces;
using flights.crosscutting.Utils;
using flights.domain.Entities;
using flights.domain.Interfaces.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace flights.application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly INotificator _notificator;

        public UserService(IUserRepository userRepository, INotificator notificator)
        {
            _userRepository = userRepository;
            _notificator = notificator;
        }

        public string Authenticate(LoginDTO login)
        {
            var user = _userRepository.GetByUsername(login.Username);

            if (user == null)
            {
                _notificator.notify("Usuário não encontrado.");
                return string.Empty;
            }

            var passwordHash = Hash.GenerateHashMD5(login.Password);

            if (user.Active)
            {
                if (user.Password == passwordHash)
                {
                    return GenerateToken(user);
                }
                else
                {
                    _notificator.notify("Senha incorreta.");
                    return string.Empty;
                }
            }
            else
            {
                _notificator.notify("Usuário inativo.");
                return string.Empty;
            }
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes("43e4dbf0-52ed-4203-895d-42b586496bd4");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Username),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
