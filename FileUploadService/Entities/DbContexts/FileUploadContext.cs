using Microsoft.EntityFrameworkCore;
using FileUploadService.Entities;

namespace FileUploadService.Entities.DbContexts;

public class FileUploadContext : DbContext
{
    public FileUploadContext(DbContextOptions<FileUploadContext> options)
        : base(options) { }

    public DbSet<FileUpload> FileUpload => Set<FileUpload>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.Entity<FileUpload>().HasKey(f => f.Id);
    }
}