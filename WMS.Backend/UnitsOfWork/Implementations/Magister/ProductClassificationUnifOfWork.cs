using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class ProductClassificationUnifOfWork : IProductClassificationUnitOfWork
    {
        private readonly IProductClassificationRepository _repos;
        public ProductClassificationUnifOfWork(IProductClassificationRepository repos)
        {
            _repos = repos;
        }
        public Task<ActionResponse<ProductClassification>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<ProductClassification>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<ProductClassification>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<ProductClassification>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<ProductClassification>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<ProductClassification>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<ProductClassification>> AddAsync(ProductClassification model, long Id_Local) => _repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<ProductClassification>>> AddListAsync(List<ProductClassification> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<ProductClassification>> UpdateAsync(ProductClassification model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);

        public Task<ActionResponse<ProductClassification>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<ProductClassification>> DeleteAsync(long id, long Id_local) => _repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<ProductClassification>> DeleteFullAsync(long id) => _repos.DeleteFullAsync(id);
    }
}
