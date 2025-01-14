using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class ProductClassificationDetailUnifOfWork : IProductClassificationDetailUnitOfWork
    {
        private readonly IProductClassificationDetailRepository _repos;
        public ProductClassificationDetailUnifOfWork(IProductClassificationDetailRepository repos)
        {
            _repos = repos;
        }
        public Task<ActionResponse<ProductClassificationDetail>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<ProductClassificationDetail>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<ProductClassificationDetail>> AddAsync(ProductClassificationDetail model, long Id_Local) => _repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<ProductClassificationDetail>>> AddListAsync(List<ProductClassificationDetail> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<ProductClassificationDetail>> UpdateAsync(ProductClassificationDetail model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);

        public Task<ActionResponse<ProductClassificationDetail>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<ProductClassificationDetail>> DeleteAsync(long id, long Id_local) => _repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<ProductClassificationDetail>> DeleteFullAsync(long id) => _repos.DeleteFullAsync(id);
    }
}
