using EventAssist.Models.Records;

namespace EventAssist.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        RoleRecord GetRoleByName(string roleName);
    }
}
