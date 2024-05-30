using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;

namespace Backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    public static User user = new User();
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    [HttpPost("registrieren")]
    public ActionResult<User> Registrieren(UserDto request)
    {
      string passwortHash =
          BCrypt.Net.BCrypt.HashPassword(request.Password);

      user.Username = request.Username;
      user.PasswordHash = passwortHash;

      return Ok(user);
    }

    [HttpPost("login")]
    public ActionResult<User> Login(UserDto request)
    {
      if (user.Username != request.Username)
      {
        return Unauthorized("Benutzername oder Passwort falsch!");
      }

      if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
      {
        return Unauthorized("Benutzername oder Passwort falsch!");
      }

      string token = ErstelleToken(user);

      return Ok(token);
    }

    private string ErstelleToken(User user)
    {
      List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

      // Hole den Schlüssel aus der Konfiguration
      var schluesselString = _configuration.GetSection("AppSettings:Token").Value;
      if (string.IsNullOrEmpty(schluesselString) || schluesselString.Length < 64)
      {
        throw new ArgumentException("Der Schlüssel muss mindestens 64 Zeichen lang sein.");
      }

      var schluessel = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(schluesselString));

      var anmeldeinformationen = new SigningCredentials(schluessel, SecurityAlgorithms.HmacSha512Signature);

      var token = new JwtSecurityToken(
          claims: claims,
          expires: DateTime.Now.AddDays(1),
          signingCredentials: anmeldeinformationen
      );

      var jwt = new JwtSecurityTokenHandler().WriteToken(token);
      return jwt;
    }
  }

}
