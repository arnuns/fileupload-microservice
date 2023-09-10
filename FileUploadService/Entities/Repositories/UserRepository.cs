using Microsoft.EntityFrameworkCore;
using FileUploadService.Entities;
using FileUploadService.Entities.DbContexts;

namespace FileUploadService.Entities.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<RefreshToken?> GetUserRefreshTokenAsync(int userId);
    Task<RefreshToken?> GetUserRefreshTokenAsync(string refreshToken);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    void UpdateRefreshToken(RefreshToken refreshToken);
    void RemoveRefreshToken(RefreshToken refreshToken);
}

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(FileUploadContext context) : base(context) { }

    public async Task AddAsync(User user)
    {
        await _context.User.AddAsync(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.User.FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public async Task<RefreshToken?> GetUserRefreshTokenAsync(int userId)
        => await _context.RefreshToken.FirstOrDefaultAsync(rt => rt.UserId == userId);

    public async Task<RefreshToken?> GetUserRefreshTokenAsync(string refreshToken)
        => await _context.RefreshToken.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        => await _context.RefreshToken.AddAsync(refreshToken);

    public void UpdateRefreshToken(RefreshToken refreshToken)
    {
        _context.RefreshToken.Update(refreshToken);
    }
    public void RemoveRefreshToken(RefreshToken refreshToken)
    {
        _context.RefreshToken.Remove(refreshToken);
    }
}