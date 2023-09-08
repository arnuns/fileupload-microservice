using Microsoft.EntityFrameworkCore;
using FileUploadService.Entities;

namespace FileUploadService.Entities.DbContexts;

public class FileContext : DbContext
{
    public FileContext(DbContextOptions<FileContext> options)
        : base(options) { }

    public DbSet<FileUpload> FileUpload => Set<FileUpload>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.Entity<FileUpload>().HasKey(f => f.Id);
    }
}