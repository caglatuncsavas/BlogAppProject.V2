namespace BlogApp.WebApi.DTOs;

public class LoginResponseDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();

}
