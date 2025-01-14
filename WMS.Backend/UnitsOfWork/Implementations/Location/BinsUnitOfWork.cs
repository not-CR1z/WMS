using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Location
{
    public class BinsUnitOfWork: IBinsUnitOfWork
    {
        private readonly IBinsRepository _repos;

        public BinsUnitOfWork(IBinsRepository repos)
        {
            _repos = repos;
        }

        public Task<ActionResponse<Bin>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<Bin>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<Bin>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<IEnumerable<Bin>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<Bin>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<Bin>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<Bin>> AddAsync(Bin model, long Id_Local) => _repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<Bin>>> AddListAsync(List<Bin> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<Bin>> DeleteAsync(long id, long Id_local) => _repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<Bin>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<Bin>> DeleteFullAsync(long id) => _repos.DeleteFullAsync(id);

        public Task<ActionResponse<Bin>> UpdateAsync(Bin model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);
    }
}
