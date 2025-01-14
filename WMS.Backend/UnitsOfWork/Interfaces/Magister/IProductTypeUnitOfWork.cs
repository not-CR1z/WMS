using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Magister
{
    public interface IProductTypeUnitOfWork
    {
        Task<ActionResponse<ProductType>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<ProductType>>> GetAsync();

        Task<ActionResponse<IEnumerable<ProductType>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<ProductType>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<ProductType>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<ProductType>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<ProductType>> AddAsync(ProductType model, long Id_Local);

        Task<ActionResponse<List<ProductType>>> AddListAsync(List<ProductType> list, long Id_Local);

        Task<ActionResponse<ProductType>> UpdateAsync(ProductType model, long Id_Local);

        Task<ActionResponse<ProductType>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<ProductType>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<ProductType>> DeleteFullAsync(long id);
    }
}
