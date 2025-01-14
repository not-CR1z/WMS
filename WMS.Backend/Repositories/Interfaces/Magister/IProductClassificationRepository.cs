using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Magister
{
    public interface IProductClassificationRepository
    {
        Task<ActionResponse<ProductClassification>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<ProductClassification>>> GetAsync();

        Task<ActionResponse<IEnumerable<ProductClassification>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<ProductClassification>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<ProductClassification>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<ProductClassification>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<ProductClassification>> AddAsync(ProductClassification model, long Id_Local);

        Task<ActionResponse<List<ProductClassification>>> AddListAsync(List<ProductClassification> list, long Id_Local);

        Task<ActionResponse<ProductClassification>> UpdateAsync(ProductClassification model, long Id_Local);

        Task<ActionResponse<ProductClassification>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<ProductClassification>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<ProductClassification>> DeleteFullAsync(long id);

    }
}
