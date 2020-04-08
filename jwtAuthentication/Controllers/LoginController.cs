using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace jwtAuthentication.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

	    private IConfiguration _Configuration;

	    public LoginController(IConfiguration configuration)
	    {
		    _Configuration = configuration;
	    }

			[Authorize]
			[HttpPost("Post")]
	    public ActionResult<IEnumerable<string>> Post()
	    {
		    return new[] {"value 1", "value 4", "value 3", "value 2"};
	    }

	    public IActionResult Login(String username, String password) {
		    cUser login = new cUser();
		    login.Username = username;
		    login.Password = password;
		    IActionResult response = Unauthorized();//set our reponse to unauthorize
		    var user = AuthenticateUser(login);//calling mock method
		    if (user != null) {
			    var tokenStr = GenerateJsonWebToken(user);
			    response = Ok(new { token = tokenStr });
		    }
		    return response;
	    }

			//mock authentication method
	    private cUser AuthenticateUser(cUser login)
	    {
		    if (login.Username == "admin" && login.Password == "admin")
		    {
			    login.Name = "Admin";
			    return login;
		    }

		    return null;
	    }


	    //method to generate web token
		private String GenerateJsonWebToken(cUser user) {
		    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["jwt:Key"]));
		    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		    var claims = new[]{
			    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
			    new Claim(JwtRegisteredClaimNames.Email, user.Password),
			    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		    };
		    var token = new JwtSecurityToken(
			    issuer: _Configuration["jwt:Issuer"],
			    audience: _Configuration["jwt:Issuer"],
			    claims,
			    expires: DateTime.Now.AddDays(1),
			    signingCredentials: credentials);
		    var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
		    return encodeToken;
	    }

	}
}