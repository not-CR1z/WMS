using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Magister
{
    public interface IUMRepository
    {
        Task<ActionResponse<UM>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<UM>>> GetAsync();

        Task<ActionResponse<IEnumerable<UM>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<UM>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<UM>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<UM>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<UM>> AddAsync(UM model, long Id_Local);

        Task<ActionResponse<List<UM>>> AddListAsync(List<UM> list, long Id_Local);

        Task<ActionResponse<UM>> UpdateAsync(UM model, long Id_Local);

        Task<ActionResponse<UM>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<UM>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<UM>> DeleteFullAsync(long id);

    }
}
