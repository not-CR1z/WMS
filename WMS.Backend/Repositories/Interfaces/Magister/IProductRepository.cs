using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Magister
{
    public interface IProductRepository
    {
        Task<ActionResponse<Product>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<Product>>> GetAsync();

        Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Product>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Product>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<Product>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<Product>> AddAsync(Product model, long Id_Local);

        Task<ActionResponse<List<Product>>> AddListAsync(List<Product> list, long Id_Local);

        Task<ActionResponse<Product>> DeleteClasificationAsync(long id);

        Task<ActionResponse<Product>> DeleteAsync(long id, long Id_local);

        Task<ActionResponse<Product>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<Product>> DeleteFullAsync(long id);

        Task<ActionResponse<Product>> UpdateAsync(Product model, long Id_Local);

    }
}
