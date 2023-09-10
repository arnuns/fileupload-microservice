using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FileUploadService.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    [JsonIgnore]
    public string? Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    [NotMapped]
    public string? Token { get; set; }
    [NotMapped]
    public string? RefreshToken { get; set; }
}

public class RefreshToken
{
    [Key]
    public string Token { get; set; } = default!;
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
    public DateTime? ExpiryDate { get; set; }
}