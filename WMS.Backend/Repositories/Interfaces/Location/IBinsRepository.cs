using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Location
{
    public interface IBinsRepository
    {
        Task<ActionResponse<Bin>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<Bin>>> GetAsync();

        Task<ActionResponse<IEnumerable<Bin>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Bin>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Bin>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<Bin>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<Bin>> AddAsync(Bin model, long Id_Local);
        Task<ActionResponse<List<Bin>>> AddListAsync(List<Bin> list, long Id_Local);

        Task<ActionResponse<Bin>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<Bin>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<Bin>> DeleteFullAsync(long id);

        Task<ActionResponse<Bin>> UpdateAsync(Bin model, long Id_Local);
    }
}
