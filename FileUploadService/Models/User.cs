namespace FileUploadService.Models;

public class Login
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class Refresh
{
    public string? RefreshToken { get; set; }
}