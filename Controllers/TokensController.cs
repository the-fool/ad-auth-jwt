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
    public TokensController(UserService _userService) {
      userService = _userService;
    }

    [HttpGet]
    [Authorize]
    public ActionResult<string> Get([FromQuery(Name="rdr")] string redirect) {
      var user = userService.WhoAmI(HttpContext);
      var token = JsonConvert.SerializeObject(user);
      var url = $"{redirect}?token={token}";
      return new RedirectResult(url, false);
    }

    [HttpGet("test")]
    [Authorize]
    public ActionResult<string> Test([FromQuery(Name="rdr")] string redirect) {
      var user = userService.WhoAmI(HttpContext);
      return JsonConvert.SerializeObject(user);
    }
    
  }
}