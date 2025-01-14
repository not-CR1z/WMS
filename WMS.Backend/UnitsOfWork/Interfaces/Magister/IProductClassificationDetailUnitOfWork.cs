using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Magister
{
    public interface IProductClassificationDetailUnitOfWork
    {
        Task<ActionResponse<ProductClassificationDetail>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> GetAsync();

        Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<ProductClassificationDetail>> AddAsync(ProductClassificationDetail model, long Id_Local);

        Task<ActionResponse<List<ProductClassificationDetail>>> AddListAsync(List<ProductClassificationDetail> list, long Id_Local);

        Task<ActionResponse<ProductClassificationDetail>> UpdateAsync(ProductClassificationDetail model, long Id_Local);

        Task<ActionResponse<ProductClassificationDetail>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<ProductClassificationDetail>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<ProductClassificationDetail>> DeleteFullAsync(long id);
    }
}
