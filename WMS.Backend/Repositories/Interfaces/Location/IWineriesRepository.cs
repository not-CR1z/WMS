using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Location
{
    public interface IWineriesRepository
    {
        Task<ActionResponse<Winery>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<Winery>>> GetAsync();

        Task<ActionResponse<IEnumerable<Winery>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Winery>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Winery>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<Winery>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<Winery>> AddAsync(Winery model, long Id_Local);
        Task<ActionResponse<List<Winery>>> AddListAsync(List<Winery> list, long Id_Local);

        Task<ActionResponse<Winery>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<Winery>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<Winery>> DeleteFullAsync(long id);

        Task<ActionResponse<Winery>> UpdateAsync(Winery model, long Id_Local);
    }
}
