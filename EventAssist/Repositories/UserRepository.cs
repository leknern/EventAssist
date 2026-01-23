using EventAssist.Contexts;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventAssist.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public UserRecord GetUserById(int userId)
        {
            return context.Users
                .Include(u => u.Roles)
                .First(u => u.Id == userId);
        }

        public UserRecord GetUserByPwdResetToken(string token)
        {
            return context.Users
                .Include(u => u.Roles)
                .First(u => u.PasswordResetToken == token);
        }

        public UserRecord? TryGetUser(string email)
        {
            return context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public UserRecord AddUser(UserRecord user)
        {
            context.Users.Add(user);
            context.SaveChanges();

            return user;
        }

        public UserRecord UpdateUser(UserRecord user)
        {
            context.Update(user);
            context.SaveChanges();

            return user;
        }
    }
}
