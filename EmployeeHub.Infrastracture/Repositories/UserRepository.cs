using EmployeeHub.Application.Data;
using EmployeeHub.Domain.Entities;
using EmployeeHub.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EmployeeHub.Infrastracture.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
