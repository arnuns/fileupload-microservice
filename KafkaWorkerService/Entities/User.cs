using System;

namespace KafkaWorkerService.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}