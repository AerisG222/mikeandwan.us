using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Identity
{
    public interface IUserRepository
    {
        Task<List<State>> GetStatesAsync();
        Task<List<Country>> GetCountriesAsync();
        Task<MawUser> GetUserAsync(string username);
        Task<MawUser> GetUserAsync(short id);
		Task<MawUser> GetUserByEmailAsync(string email);
        Task<List<string>> GetRoleNamesForUserAsync(string username);
        Task<bool> UpdateUserAsync(MawUser updatedUser);
		Task<bool> UpdateUserPasswordAsync(MawUser user);
        Task<bool> AddLoginHistoryAsync(string username, short loginActivityTypeId, short loginAreaId);
        Task<bool> AddExternalLoginHistoryAsync(string email, short loginActivityTypeId, short loginAreaId);
        Task<List<UserAndLastLogin>> GetUsersToManageAsync();
        Task<List<string>> GetAllUsernamesAsync();
        Task<List<string>> GetAllRoleNamesAsync();
        Task<int> AddUserAsync(MawUser user);
        Task<int> RemoveUserAsync(string username);
        Task<List<MawUser>> GetUsersInRoleAsync(string roleName);
        Task<bool> CreateRoleAsync(string roleName, string description);
        Task<bool> RemoveRoleAsync(string roleName);
        Task<bool> AddUserToRoleAsync(string username, string roleName);
        Task<bool> RemoveUserFromRoleAsync(string username, string roleName);
		Task<bool> SetSecurityStampAsync(string username, string securityStamp);
		Task<MawRole> GetRoleAsync(string roleName);
		Task<MawRole> GetRoleAsync(short id);
    }
}
    