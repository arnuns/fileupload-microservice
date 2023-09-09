namespace FileUploadService.Models;

public class Login
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class RefreshToken
{
    public string? Token { get; set; }
    public int? UserId { get; set; }
    public DateTime ExpiryDate { get; set; }
}