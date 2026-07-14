namespace EventManagement.Repositories;

public interface IUserRepository
{
    Task<Models.User?> GetByEmailAsync(string email);
    Task<Models.User?> GetByIdAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(Models.User user);
    Task<List<Models.User>> GetAllAsync();
    Task UpdateAsync(Models.User user);
}
