using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class ProductUnitOfWork:IProductUnitOfWork
    {
        private readonly IProductRepository _repos;

        public ProductUnitOfWork(IProductRepository repos)
        {
            _repos = repos;
        }

        public Task<ActionResponse<Product>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<Product>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<IEnumerable<Product>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<Product>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<Product>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<Product>> AddAsync(Product model, long Id_Local) => _repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<Product>>> AddListAsync(List<Product> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<Product>> DeleteClasificationAsync(long id) => _repos.DeleteClasificationAsync(id);

        public Task<ActionResponse<Product>> DeleteAsync(long id, long Id_local) => _repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<Product>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<Product>> DeleteFullAsync(long id) => _repos.DeleteFullAsync(id);

        public Task<ActionResponse<Product>> UpdateAsync(Product model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);
    }
}
