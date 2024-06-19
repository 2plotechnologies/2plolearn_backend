using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;


namespace LMS.Backend.Helpers
{
    public class JwtService
    {
        private string SecureKey = "Of9ypI5wjAcVOq56k51NQmtBmOn4yVmK";
        public string Generate(int id){
            var symmetricSecureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecureKey));
            var credentials = new SigningCredentials(symmetricSecureKey, SecurityAlgorithms.HmacSha256Signature);
            var header = new JwtHeader(credentials);
            var payload = new JwtPayload(id.ToString(), null, null, null, DateTime.Today.AddDays(1)); // 1 day
            var securityToken = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricSecureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecureKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecureKey,
            };

            try
            {
                SecurityToken validatedToken;
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true; // El token es válido
            }
            catch (Exception)
            {
                return false; // El token es inválido
            }
        }
    }
}