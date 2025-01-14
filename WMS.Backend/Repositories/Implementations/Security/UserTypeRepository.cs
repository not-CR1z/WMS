using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Security
{
    public class UserTypeRepository : GenericRepository<UserType>, IUserTypeRepository
    {
        private readonly DataContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserTypeRepository(DataContext context, RoleManager<IdentityRole> roleManager) : base(context)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<ActionResponse<UserType>> PostAsync(UserType Model)
        {
            try
            {
                await _context.UserTypes.AddAsync(Model);
                var roleExists = await _roleManager.RoleExistsAsync(Model.Name);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = Model.Name
                    });
                }
                return new ActionResponse<UserType>
                {
                    WasSuccess = true,
                    Result = Model,
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return new ActionResponse<UserType>
                        {
                            WasSuccess = false,
                            Message = "Registro duplicado",
                        };
                    }
                }

                return new ActionResponse<UserType>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<UserType>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public override async Task<ActionResponse<IEnumerable<UserType>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.UserTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<UserType>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.UserTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
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
