using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Location
{
    public class WineriesUnitOfWork: IWineriesUnitOfWork
    {
        private readonly IWineriesRepository _repos;

        public WineriesUnitOfWork(IWineriesRepository repos)
        {
            _repos = repos;
        }

        public Task<ActionResponse<Winery>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<Winery>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<Winery>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<IEnumerable<Winery>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<Winery>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<Winery>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<Winery>> AddAsync(Winery model, long Id_Local) => _repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<Winery>>> AddListAsync(List<Winery> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<Winery>> DeleteAsync(long id, long Id_local) => _repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<Winery>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<Winery>> DeleteFullAsync(long id) => _repos.DeleteFullAsync(id);

        public Task<ActionResponse<Winery>> UpdateAsync(Winery model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);
    }
}
