using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using webapi.DTO;
using webapi.Models;
using webapi.Services;
using ZstdSharp.Unsafe;

namespace webapi.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _usersService;
        public UserController(ILogger<UserController> logger, UserService usersService)
        {
            _usersService = usersService;
        }
        [HttpPost("signup")]
        public async Task<ActionResult> SignUp([FromBody] UserDTO user)
        {
            try
            {
                var euser = await _usersService.GetByEmailAsync(user.Email);

                if (euser != null)
                {
                    return BadRequest(new { msg = "Korisnik sa navedenom email adresom vec postoji" });
                }

                await _usersService.CreateAsync(user);
                return Ok(new { msg = "Uspesna registracija" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Doslo je do greske" });
            }
        }

        [HttpPost("signin")]
        public async Task<ActionResult> SignIn([FromBody] AuthCredsDTO creds)
        {
            var user = await _usersService.GetByEmailAsync(creds.Email);

            if (user == null || !_usersService.VerifyPassword(creds.Password, user.PasswordHash))
            {
                return Unauthorized(new { msg = "Pogresan email ili lozinka" });
            }

            return Ok(new
            {
                User = user,
                AuthToken = _usersService.CreateToken(user)
            });
        }
    }
}