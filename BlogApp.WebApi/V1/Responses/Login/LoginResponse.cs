namespace BlogApp.WebApi.V1.Responses.Login;

public class LoginResponse
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();

}
