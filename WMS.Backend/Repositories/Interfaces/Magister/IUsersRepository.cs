using Microsoft.AspNetCore.Identity;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Magister
{
    public interface IUsersRepository
    {
        Task<User> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> DeleteUserAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string password);

        Task<IdentityResult> ChangeEmailAsync(User user, string email);

        Task<List<UserType>> GetRoleUserAsync(long UserIdLocal);

        Task CheckRoleAsync(string roleName);

        Task<ActionResponse<bool>> UserToRoleAsync(User user, List<long> UserTypeIds);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> LoginAsync(LoginDTO model);

        Task LogoutAsync();
        Task<User> GetAsync(string Id);

        Task<User> GetIdLocalAsync(long Id);

        Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

    }
}
