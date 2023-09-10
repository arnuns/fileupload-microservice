using Microsoft.EntityFrameworkCore;
using FileUploadService.Entities;

namespace FileUploadService.Entities.DbContexts;

public class FileUploadContext : DbContext
{
    public FileUploadContext(DbContextOptions<FileUploadContext> options)
        : base(options) { }

    public DbSet<User> User => Set<User>();
    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}