using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Location
{
    public interface ISubWineriesUnitOfWork
    {
        Task<ActionResponse<SubWinery>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<SubWinery>>> GetAsync();

        Task<ActionResponse<IEnumerable<SubWinery>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<SubWinery>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<SubWinery>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<SubWinery>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<SubWinery>> AddAsync(SubWinery model, long Id_Local);

        Task<ActionResponse<List<SubWinery>>> AddListAsync(List<SubWinery> list, long Id_Local);

        Task<ActionResponse<SubWinery>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<SubWinery>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<SubWinery>> DeleteFullAsync(long id);

        Task<ActionResponse<SubWinery>> UpdateAsync(SubWinery model, long Id_Local);
    }
}
