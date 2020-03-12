using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MAAV.DataContracts;
using MAAV.Domain.Repositories;

namespace MAAV.WebAPI.Extensions
{
    static public class AuthenticationExtension
    {
        public static string Secret { get; private set; }
        public static TimeSpan Expiration { get; private set; }
        public static void GenerateToken(this Authentication authResult)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            IdentityModelEventSource.ShowPII = true;
            var key = Encoding.UTF8.GetBytes(Secret);
            
            var roles = authResult.Roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
            roles.Add(new Claim(ClaimTypes.Name, $"{authResult.OrganisationName}.{authResult.User.Username}"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(roles),
                Expires = DateTime.UtcNow.Add(Expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            authResult.AccessToken = tokenHandler.WriteToken(token);
            authResult.Expiration = token.ValidTo;
        }
        
        public static void AddBearerTokenValidation(this IServiceCollection  serviceCollection, string key, TimeSpan expiration)
        {
            serviceCollection
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            
            Secret = key;
            Expiration = expiration;
        }
    }
}