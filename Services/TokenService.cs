using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Authy.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authy.Services {
  public class TokenService {
    UserService userService { get; }
    AppSettings appSettings { get; set; }
    public TokenService(
      UserService _userService,
      IOptions<AppSettings> _appSettings
    ) {
      userService = _userService;
      appSettings = _appSettings.Value;
    }

    public string MakeToken(HttpContext ctx) {
      var user = userService.WhoAmI(ctx);
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(appSettings.Secret);
      var claims = new Claim[] {
        new Claim(ClaimTypes.NameIdentifier, user.AccountName),
        new Claim("iss", "authy"),
        new Claim("name", user.DisplayName)
      };
      var subject = new ClaimsIdentity(claims);
      var tokenDescriptor = new SecurityTokenDescriptor {
        Subject = subject,
        Expires = DateTime.UtcNow.AddYears(10),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}