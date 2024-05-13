using BlogApp.WebApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;

    public AccountController(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }

    //POST: {apibaseurl}/api/auth/login
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        //Check Email
        var identityUser = await userManager.FindByEmailAsync(request.Email);

        if (identityUser is not null)
        {
            //Check Password
            var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, request.Password);

            if (checkPasswordResult)
            {
                var roles = await userManager.GetRolesAsync(identityUser);

                //Create a Token and Response
                //var jwtToken = tokenRepository.CreateJwtToken(identityUser, roles.ToList());
                var jwtToken =  

                var response = new LoginResponseDto()
                {
                    Email = request.Email,
                    Roles = roles.ToList(),
                    Token = jwtToken
                };

                return Ok(response);
            }
        }
        ModelState.AddModelError("", "Email or Password Incorrect");

        return ValidationProblem(ModelState);
    }

    //POST: {apibaseurl}/api/auth/register
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        //Create IdentityUser object

        var user = new IdentityUser
        {
            UserName = request.Email?.Trim(),
            Email = request.Email?.Trim()
        };

        //Create User
        var IdentityResult = await userManager.CreateAsync(user, request.Password);

        if (IdentityResult.Succeeded)
        {
            //Add Role to user (reader)
            IdentityResult = await userManager.AddToRoleAsync(user, "Reader");

            if (IdentityResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                if (IdentityResult.Errors.Any())
                {
                    foreach (var error in IdentityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
        }
        else
        {
            if (IdentityResult.Errors.Any())
            {
                foreach (var error in IdentityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

        }

        return ValidationProblem(ModelState);
    }
}
