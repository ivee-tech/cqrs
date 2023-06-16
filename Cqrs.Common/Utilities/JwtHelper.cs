using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Common.Utilities
{
    public static class JwtHelper
    {
        public static JwtSecurityToken ParseToken(string tokenData)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(tokenData))
            {
                var token = tokenHandler.ReadJwtToken(tokenData);
                return token;
            }
            throw new Exception("Unable to parse token.");
        }

        public static async Task<SecurityToken> ValidateToken(string token, string endpointBaseAddress, string audience, bool validateLifetime = true)
        {
            var discoveryEndpoint = $"{endpointBaseAddress}/.well-known/openid-configuration";
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(discoveryEndpoint, new OpenIdConnectConfigurationRetriever());
            var openIdConfig = await configManager.GetConfigurationAsync();
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = openIdConfig.Issuer,
                ValidAudience = audience,
                IssuerSigningKeys = openIdConfig.SigningKeys,
                ValidateLifetime = validateLifetime
            }, out SecurityToken validatedToken);
            return validatedToken;
        }
    }
}
