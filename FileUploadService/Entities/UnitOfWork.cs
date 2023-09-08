namespace FileUploadService.Entities.DbContexts;

public interface IUnitOfWork
{
    Task CompleteAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly FileUploadContext _context;

    public UnitOfWork(FileUploadContext context)
    {
        _context = context;
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }
}