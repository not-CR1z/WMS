using Microsoft.AspNetCore.Mvc;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Security
{
    public interface IUserTypeRepository
    {

        Task<ActionResponse<UserType>> PostAsync(UserType Model);
        Task<ActionResponse<IEnumerable<UserType>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
    }
}
