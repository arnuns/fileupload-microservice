using FileUploadService.Entities.DbContexts;

namespace FileUploadService.Entities.Repositories;

public abstract class BaseRepository
{
    protected readonly FileUploadContext _context;

    public BaseRepository(FileUploadContext context)
    {
        _context = context;
    }
}