using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using NuGet.Versioning;
using System.Linq.Expressions;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Magister
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UsersRepository(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<ActionResponse<bool>> UserToRoleAsync(User user, List<long> UserTypeIds)
        {
            try
            {
                var userTypeUsersdb = await _context.UserTypeUsers.Where(w => w.UserId == user.Id).Include(I => I.UserType).ToListAsync();
                var UserTypes =
                    (from item in _context.UserTypes.AsEnumerable()
                     join item2 in UserTypeIds on item.Id equals item2
                     select item).ToList();


                foreach (var item in UserTypes)
                {
                    var usertypedb = userTypeUsersdb.Where(w => w.UserTypeId == item.Id).FirstOrDefault();
                    if (usertypedb == null)
                    {
                        await _userManager.AddToRoleAsync(user, item.Name);
                        var userTypeUser = new UserTypeUser
                        {
                            UserId = user.Id,
                            UserIdLocal = user.Id_Local,
                            UserTypeId = item.Id,
                        };
                        await _context.UserTypeUsers.AddAsync(userTypeUser);
                        await _context.SaveChangesAsync();
                    }
                }
                userTypeUsersdb = await _context.UserTypeUsers.Where(w => w.UserId == user.Id).Include(I => I.UserType).ToListAsync();
                foreach (var item in userTypeUsersdb)
                {
                    var usertype = UserTypes.Where(w => w.Id == item.UserTypeId).FirstOrDefault();
                    if (usertype == null)
                    {
                        _context.UserTypeUsers.Remove(item);
                        List<string> roles = new List<string> { item.UserType!.Name };
                        await _userManager.RemoveFromRolesAsync(user, roles);
                        _context.SaveChanges();
                    }
                }

                //var add = UserTypeUsers.ExceptBy(userTypeUsersdb.Select(x => x.Id), x => x.Id).ToList();
                //var add=
                //    (from item in UserTypeUsers.Except(userTypeUsersdb).AsEnumerable()
                //    select new UserTypeUser
                //    {
                //        UserId = user.Id,
                //        UserIdLocal=user.Id_Local,
                //        UserTypeId= item.Id,
                //        UserType = item.UserType,
                //    }).ToList();
                //var remove = userTypeUsersdb.ExceptBy(UserTypeUsers.Select(x => x.Id), x => x.Id).ToList();
                //var remove=
                //    (from item in userTypeUsersdb.Except(UserTypeUsers).AsEnumerable()
                //    select new UserTypeUser
                //    {
                //        UserId = user.Id,
                //        UserIdLocal = user.Id_Local,
                //        UserTypeId = item.Id,
                //        UserType=item.UserType,
                //    }).ToList();

                //if (add.Count()>0)
                //{
                //    foreach (var item in add)
                //    {
                //        await _userManager.AddToRoleAsync(user, item.UserType!.Name);
                //        await _context.UserTypeUsers.AddAsync(item);
                //    }                    
                //    await _context.SaveChangesAsync();
                //}
                //if( remove.Count()>0)
                //{
                //    foreach (var item in remove)
                //    {
                //        _context.UserTypeUsers.Remove(item);
                //        List<string> roles = new List<string> { item.UserType!.Name };
                //        await _userManager.RemoveFromRolesAsync(user, roles);
                //    }                    
                //    _context.SaveChanges();
                //}
                var actionResponse = new ActionResponse<bool>
                {
                    Result = true,
                    WasSuccess = true,
                    Message = "Roles modificados correctamente"
                };
                return actionResponse;
            }
            catch (Exception ex)
            {
                var actionResponse = new ActionResponse<bool>
                {
                    Result = false,
                    WasSuccess = false,
                    Message = ex.Message
                };
                return actionResponse;
            }
        }

        public async Task<ActionResponse<bool>> RemoveUserToRoleAsync(User user, string roleName)
        {
            List<string> roles = new List<string> { roleName };
            await _userManager.RemoveFromRolesAsync(user, roles);
            var userType = await _context.UserTypes.Where(w => w.Name == roleName).FirstOrDefaultAsync();
            var userTypeUser = await _context.UserTypeUsers.Where(w => w.UserIdLocal == user.Id_Local && w.UserTypeId == userType!.Id).FirstOrDefaultAsync();
            _context.UserTypeUsers.Remove(userTypeUser!);
            await _context.SaveChangesAsync();
            var actionResponse = new ActionResponse<bool>
            {
                Result = true,
                WasSuccess = true,
                Message = "Rol Eliminado correctamente"
            };
            return actionResponse;
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string password)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> ChangeEmailAsync(User user, string email)
        {
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, email);
            return await _userManager.ChangeEmailAsync(user, email, token);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<List<UserType>> GetRoleUserAsync(long UserIdLocal)
        {
            var userTypeUsers = await _context.UserTypeUsers.Where(w => w.UserIdLocal == UserIdLocal).Include(I => I.UserType).ToListAsync();
            var userTypes = (from item in userTypeUsers.AsEnumerable()
                             select new UserType
                             {
                                 Id = item.UserTypeId,
                                 Name = item.UserType!.Name,
                             }).ToList();
            return userTypes;
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.DocumentTypeUser)
                .FirstOrDefaultAsync(x => x.Email == email);
            return user!;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<User> GetAsync(string Id)
        {
            var user = await _context.Users
                .Include(u => u.DocumentTypeUser)
                .FirstOrDefaultAsync(x => x.Id == Id);
            return user!;
        }

        public async Task<User> GetIdLocalAsync(long Id)
        {
            var user = await _context.Users
                .Where(w => w.Id_Local == Id)
                .Include(u => u.DocumentTypeUser)
                .FirstOrDefaultAsync();
            return user!;
        }

        public async Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable =
                queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower())
                || x.LastName.ToLower().Contains(pagination.Filter.ToLower())
                || x.Document.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<User>>
            {
                WasSuccess = true,
                Result = await queryable
                .Include(u => u.DocumentTypeUser)
                .OrderBy(x => x.LastName)
                .Paginate(pagination)
                .ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable =
                queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower())
                || x.LastName.ToLower().Contains(pagination.Filter.ToLower())
                || x.Document.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

    }
}
