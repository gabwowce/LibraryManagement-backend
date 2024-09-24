using LibraryManagement.Helpers;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.Services
{
    public class AuthenticationService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly string _jwtSecret;


        public AuthenticationService(IMemberRepository memberRepository, IConfiguration configuration)
        {
            _memberRepository = memberRepository;
            _jwtSecret = configuration["Jwt:Secret"];
        }

        public (string token, MemberDto member) Authenticate(string username, string password)
        {
            var member = _memberRepository.GetByUsername(username);
            if (member == null || !PasswordHelper.VerifyPassword(password, member.Password))
            {
                return (null, null);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, member.Username),
            new Claim(ClaimTypes.Role, member.Role)
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            
            var memberDto = new MemberDto
            {
                Name = member.Name,
                Surname = member.Surname,
                Role = member.Role
            };

            return (jwtToken, memberDto);
        }

    }
}
