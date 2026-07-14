using EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    public UserRepository(ApplicationDbContext db) { _db = db; }

    public Task<Models.User?> GetByEmailAsync(string email) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public Task<Models.User?> GetByIdAsync(int id) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);

    public Task<bool> EmailExistsAsync(string email) =>
        _db.Users.AnyAsync(u => u.Email == email);

    public async Task AddAsync(Models.User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Models.User>> GetAllAsync() =>
        await _db.Users.AsNoTracking().OrderBy(u => u.UserId).ToListAsync();

    public async Task UpdateAsync(Models.User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}
