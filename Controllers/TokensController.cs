using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Authy.Controllers {

  [Route("api/[controller]")]
  [ApiController]
  public class TokensController : ControllerBase {
    UserService userService;
    TokenService tokenService;
    public TokensController(
      UserService _userService,
      TokenService _tokenService
    ) {
      userService = _userService;
      tokenService = _tokenService;
    }

    [HttpGet]
    [Authorize]
    public ActionResult<string> Get([FromQuery(Name = "rdr")] string redirect) {
      var token = tokenService.MakeToken(HttpContext);
      var url = $"{redirect}?token={token}";
      return new RedirectResult(url, false);
    }

    [HttpGet("user")]
    [Authorize]
    public ActionResult<string> TestUser() {
      var user = userService.WhoAmI(HttpContext);
      return JsonConvert.SerializeObject(user);
    }

    [HttpGet("token")]
    [Authorize]
    public ActionResult<string> TestToken() {
      var token = tokenService.MakeToken(HttpContext);
      return token;
    }
  }
}