using WMS.Share.DTOs;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Security
{
    public interface IFormUserTypeUnitOfWork
    {
        Task<ActionResponse<FormParent>> GetFormParentIdAsync(long Id);
        Task<ActionResponse<Form>> GetFormIdAsync(long Id);
        Task<ActionResponse<IEnumerable<FormParent>>> GetFormParentAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetFormParentTotalPagesAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Form>>> GetFormAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetFormTotalPagesAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<FormUserType>>> GetFormUserTypeAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetFormUserTypeTotalPagesAsync(PaginationDTO pagination);
        Task<ActionResponse<List<FormParentDTO>>> GetFormParentUser(long Id_local);
    }
}
