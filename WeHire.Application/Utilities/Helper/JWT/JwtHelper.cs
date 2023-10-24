using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WeHire.Domain.Entities;

namespace WeHire.Application.Utilities.Helper
{
    public interface IJwtHelper
    {
        string generateJwtToken(Role role, int id);
    }
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _config;
        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string generateJwtToken(Role role, int id)
        {
            string securityKey = _config["JWT:Key"];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var claim = new[]{
                new Claim("Id", id.ToString()),
                new Claim(ClaimTypes.Role, role.RoleName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: signingCredentials,
                claims: claim
            );

            //var tokenHandler = new JwtSecurityTokenHandler();

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(claim),
            //    Expires = DateTime.Now.AddDays(1),
            //    SigningCredentials = signingCredentials
            //};

            //var tokens = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            //var jwtToken = tokenHandler.WriteToken(tokens);

            //return jwtToken;

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
