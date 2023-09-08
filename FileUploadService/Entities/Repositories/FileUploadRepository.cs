using Microsoft.EntityFrameworkCore;
using FileUploadService.Entities;
using FileUploadService.Entities.DbContexts;

namespace FileUploadService.Entities.Repositories;

public interface IFileUploadRepository
{
    Task AddAsync(FileUpload fileUpload);
    Task<FileUpload?> GetAsync(int id);
    void Remove(FileUpload fileUpload);
    void Update(FileUpload fileUpload);
}

public class FileUploadRepository : BaseRepository, IFileUploadRepository
{
    public FileUploadRepository(FileUploadContext context) : base(context) { }

    public async Task AddAsync(FileUpload fileUpload)
    {
        await _context.FileUpload.AddAsync(fileUpload);
    }

    public async Task<FileUpload?> GetAsync(int id)
    {
        return await _context.FileUpload.FirstOrDefaultAsync(f => f.Id == id);
    }

    public void Remove(FileUpload fileUpload)
    {
        _context.FileUpload.Remove(fileUpload);
    }

    public void Update(FileUpload fileUpload)
    {
        _context.FileUpload.Update(fileUpload);
    }
}