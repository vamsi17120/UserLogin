using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserLogin.Models;

namespace UserLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserContext userContext;
        private readonly IConfiguration config;
        public LoginController(UserContext userContext,IConfiguration config)
        {
            this.userContext = userContext;
            this.config = config;
        }
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var userdetails = userContext.Users.AsQueryable();
            return Ok(userdetails);
        }
        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] User Userobj)
        {
            if(Userobj == null)
            {
                return BadRequest();
            }
            else
            {
                userContext.Users.Add(Userobj);
                userContext.SaveChanges();
                return Ok(new
                {
                    StausCode = 200,
                    Message = "User Added Successfully"
                }) ;
            }
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] User userobj)
        {
            if (userobj == null)
            {
                return BadRequest();
            }
            else
            {
                var user=userContext.Users.Where(x=>x.Username == userobj.Username 
                && x.Password==userobj.Password).FirstOrDefault();
                if(user!=null && user.Password==userobj.Password)
                {
                    var token = GenerateToken(userobj.Username);
                    return Ok(new
                    {
                        StatuCode = 200,
                        Message = "Logged In Successfully",
                        UserData = userobj.Username,
                        JwtToken=token
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "User Not Found"
                    }
                        );
                }
            }
        }
        private string GenerateToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Issuer"], null, expires: System.DateTime.Now.AddMinutes(30), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
