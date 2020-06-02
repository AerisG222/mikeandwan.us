using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Identity
{
    public interface IUserRepository
    {
        Task<IEnumerable<State>> GetStatesAsync();
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task<MawUser> GetUserAsync(string username);
        Task<MawUser> GetUserAsync(short id);
        Task<MawUser> GetUserByEmailAsync(string email);
        Task<IEnumerable<string>> GetRoleNamesForUserAsync(string username);
        Task<bool> UpdateUserAsync(MawUser updatedUser);
        Task<bool> UpdateUserPasswordAsync(MawUser user);
        Task<bool> AddLoginHistoryAsync(string username, short loginActivityTypeId, short loginAreaId);
        Task<bool> AddExternalLoginHistoryAsync(string email, short loginActivityTypeId, short loginAreaId);
        Task<IEnumerable<UserAndLastLogin>> GetUsersToManageAsync();
        Task<IEnumerable<string>> GetAllUsernamesAsync();
        Task<IEnumerable<string>> GetAllRoleNamesAsync();
        Task<short> AddUserAsync(MawUser user);
        Task<long> RemoveUserAsync(string username);
        Task<IEnumerable<MawUser>> GetUsersInRoleAsync(string roleName);
        Task<bool> CreateRoleAsync(string roleName, string description);
        Task<bool> RemoveRoleAsync(string roleName);
        Task<bool> AddUserToRoleAsync(string username, string roleName);
        Task<bool> RemoveUserFromRoleAsync(string username, string roleName);
        Task<bool> SetSecurityStampAsync(string username, string securityStamp);
        Task<MawRole> GetRoleAsync(string roleName);
        Task<MawRole> GetRoleAsync(short id);
    }
}
