using EventAssist.Models.Records;

namespace EventAssist.Repositories.Interfaces
{
    public interface IUserRepository
    {
        UserRecord GetUserById(int userId);
        UserRecord GetUserByPwdResetToken(string token);
        UserRecord? TryGetUser(string email);
        UserRecord AddUser(UserRecord user);
        UserRecord UpdateUser(UserRecord user);
    }
}
