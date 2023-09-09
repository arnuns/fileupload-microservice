using Microsoft.EntityFrameworkCore;
using KafkaWorkerService.Entities;

namespace KafkaWorkerService.Entities.DbContexts;

public class FileUploadContext : DbContext
{
    public FileUploadContext(DbContextOptions<FileUploadContext> options)
        : base(options) { }

    public DbSet<FileUpload> FileUpload { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.Entity<FileUpload>().HasKey(f => f.Id);
    }
}