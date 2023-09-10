using FileUploadService.Entities;
using FileUploadService.Entities.Repositories;

namespace FileUploadService.Services;

public interface IUserService
{
    Task CreateUserAsync(string email, string password);
    Task<User?> GetByEmailAsync(string email);
    Task<RefreshToken?> GetUserRefreshTokenAsync(string refreshToken);
    Task<RefreshToken> RefreshTokenAsync(int userId, string token, DateTime expiryDate);
}


public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateUserAsync(string email, string password)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Email = email, Password = hashedPassword, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        await _userRepository.AddAsync(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _userRepository.GetByEmailAsync(email);

    public async Task<RefreshToken?> GetUserRefreshTokenAsync(string refreshToken)
        => await _userRepository.GetUserRefreshTokenAsync(refreshToken);

    public async Task<RefreshToken> RefreshTokenAsync(int userId, string token, DateTime expiryDate)
    {
        var refreshToken = await _userRepository.GetUserRefreshTokenAsync(userId);
        if (refreshToken != null)
            _userRepository.RemoveRefreshToken(refreshToken);
        refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiryDate = expiryDate
        };
        await _userRepository.AddRefreshTokenAsync(refreshToken);
        await _unitOfWork.CompleteAsync();
        return refreshToken;
    }
}