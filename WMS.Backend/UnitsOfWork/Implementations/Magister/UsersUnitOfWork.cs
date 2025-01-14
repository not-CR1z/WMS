using Microsoft.AspNetCore.Identity;
using WMS.Backend.Repositories.Interfaces;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class UsersUnitOfWork : IUsersUnitOfWork
    {
        private readonly IUsersRepository _repos;

        public UsersUnitOfWork(IUsersRepository usersRepository)
        {
            _repos = usersRepository;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password) => await _repos.AddUserAsync(user, password);

        public async Task<IdentityResult> UpdateUserAsync(User user) => await _repos.UpdateUserAsync(user);

        public async Task<IdentityResult> DeleteUserAsync(User user) => await _repos.DeleteUserAsync(user);

        public async Task<IdentityResult> ResetPasswordAsync(User user, string password) => await _repos.ResetPasswordAsync(user, password);

        public async Task<IdentityResult> ChangeEmailAsync(User user, string email) => await _repos.ChangeEmailAsync(user, email);

        public async Task<ActionResponse<bool>> UserToRoleAsync(User user, List<long> UserTypeIds) => await _repos.UserToRoleAsync(user, UserTypeIds);

        public async Task CheckRoleAsync(string roleName) => await _repos.CheckRoleAsync(roleName);

        public async Task<User> GetUserAsync(string email) => await _repos.GetUserAsync(email);

        public async Task<bool> IsUserInRoleAsync(User user, string roleName) => await _repos.IsUserInRoleAsync(user, roleName);

        public async Task<SignInResult> LoginAsync(LoginDTO model) => await _repos.LoginAsync(model);

        public async Task LogoutAsync() => await _repos.LogoutAsync();

        public async Task<User> GetAsync(string Id) => await _repos.GetAsync(Id);

        public async Task<User> GetIdLocalAsync(long Id) => await _repos.GetIdLocalAsync(Id);

        public async Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination) => await _repos.GetAsync(pagination);

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => await _repos.GetTotalPagesAsync(pagination);

        public Task<List<UserType>> GetRoleUserAsync(long UserIdLocal) => _repos.GetRoleUserAsync(UserIdLocal);


    }
}
