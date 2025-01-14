using WMS.Share.DTOs;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Security
{
    public interface IUserTypeUnitOfWork
    {
        Task<ActionResponse<UserType>> PostAsync(UserType Model);
        Task<ActionResponse<IEnumerable<UserType>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
    }
}
