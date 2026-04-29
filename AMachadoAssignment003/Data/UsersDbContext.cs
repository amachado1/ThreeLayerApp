using AMachadoAssignment003.Models;
using Microsoft.EntityFrameworkCore;

namespace AMachadoAssignment003.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<DeletedUserModel> DeletedUsers { get; set; }
    }
}
