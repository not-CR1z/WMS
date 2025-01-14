using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class UMUnifOfWork : IUMUnitOfWork
    {
        private readonly IUMRepository _repos;
        public UMUnifOfWork(IUMRepository repos)
        {
            _repos = repos;
        }
        public Task<ActionResponse<UM>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<UM>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<UM>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<UM>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<UM>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<UM>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<UM>> AddAsync(UM model, long Id_Local) => _repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<UM>>> AddListAsync(List<UM> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<UM>> UpdateAsync(UM model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);

        public Task<ActionResponse<UM>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<UM>> DeleteAsync(long id, long Id_local) => _repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<UM>> DeleteFullAsync(long id) => _repos.DeleteFullAsync(id);
    }
}
