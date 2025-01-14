using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Magister
{
    public interface IBinTypeUnitOfWork
    {
        Task<ActionResponse<BinType>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<BinType>>> GetAsync();

        Task<ActionResponse<IEnumerable<BinType>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<BinType>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<BinType>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<BinType>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<BinType>> AddAsync(BinType model, long Id_Local);

        Task<ActionResponse<List<BinType>>> AddListAsync(List<BinType> list, long Id_Local);

        Task<ActionResponse<BinType>> UpdateAsync(BinType model, long Id_Local);

        Task<ActionResponse<BinType>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<BinType>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<BinType>> DeleteFullAsync(long id);


    }
}
