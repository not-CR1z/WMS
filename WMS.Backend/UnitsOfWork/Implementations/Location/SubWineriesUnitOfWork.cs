using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Location
{
    public class SubWineriesUnitOfWork: ISubWineriesUnitOfWork
    {
        private readonly ISubWineriesRepository _repos;

        public SubWineriesUnitOfWork(ISubWineriesRepository repos)
        {
            _repos = repos;
        }

        public Task<ActionResponse<SubWinery>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<SubWinery>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<SubWinery>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<IEnumerable<SubWinery>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<SubWinery>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<SubWinery>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<SubWinery>> AddAsync(SubWinery model, long Id_Local) => _repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<SubWinery>>> AddListAsync(List<SubWinery> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<SubWinery>> DeleteAsync(long id, long Id_local) => _repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<SubWinery>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<SubWinery>> DeleteFullAsync(long id) => _repos.DeleteFullAsync(id);

        public Task<ActionResponse<SubWinery>> UpdateAsync(SubWinery model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);
    }
}
