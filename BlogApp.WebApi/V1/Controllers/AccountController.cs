using BlogApp.WebApi.V1.Requests.Login;
using BlogApp.WebApi.V1.Requests.Register;
using BlogApp.WebApi.V1.Responses.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogApp.WebApi.V1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    private string CreateToken(IdentityUser user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email ?? ""),
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["jwt:Issuer"],
            audience: _configuration["jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Check Email
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is not null)
        {
            // Check Password
            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);

            if (passwordCheck)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Create a Token and Response
              
                var token = await Task.Run(() => CreateToken(user, roles.ToList()));

                var response = new LoginResponse()
                {
                    Email = request.Email,
                    Roles = roles.ToList(),
                    Token = token
                };

                return Ok(response);
            }
        }
        ModelState.AddModelError("", "Email or Password Incorrect");

        return ValidationProblem(ModelState);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Create IdentityUser object
        var user = new IdentityUser
        {
            UserName = request.Email?.Trim(),
            Email = request.Email?.Trim()
        };

        // Create User with Password
        var identityResult = await _userManager.CreateAsync(user, request.Password);

        // Check for errors in creating user
        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return ValidationProblem(ModelState);
        }

        // Add Role to user (reader)
        identityResult = await _userManager.AddToRoleAsync(user, "Reader");

        // Check for errors in adding role
        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return ValidationProblem(ModelState);
        }

        return Ok();
    }
}
