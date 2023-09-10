using Microsoft.EntityFrameworkCore;
using KafkaWorkerService.Entities;

namespace KafkaWorkerService.Entities.DbContexts;

public class FileUploadContext : DbContext
{
    public FileUploadContext(DbContextOptions<FileUploadContext> options)
        : base(options) { }

    public DbSet<FileUpload> FileUpload => Set<FileUpload>();
    public DbSet<User> User => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<FileUpload>().HasKey(e => e.Id);
        modelBuilder.Entity<User>().HasKey(e => e.Id);
    }
}