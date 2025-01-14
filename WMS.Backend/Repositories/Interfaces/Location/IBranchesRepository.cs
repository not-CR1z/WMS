using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Location
{
    public interface IBranchesRepository
    {
        Task<ActionResponse<Branch>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<Branch>>> GetAsync();

        Task<ActionResponse<IEnumerable<Branch>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Branch>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Branch>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<Branch>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<Branch>> AddAsync(Branch model,long Id_Local);
        Task<ActionResponse<List<Branch>>> AddListAsync(List<Branch> list, long Id_Local);

        Task<ActionResponse<Branch>> DeleteAsync(long id,long Id_local);

        Task<ActionResponse<Branch>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<Branch>> DeleteFullAsync(long id);

        Task<ActionResponse<Branch>> UpdateAsync(Branch model, long Id_Local);
        
    }
}
