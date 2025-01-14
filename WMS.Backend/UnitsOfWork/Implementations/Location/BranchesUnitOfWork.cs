using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Location
{
    public class BranchesUnitOfWork : IBranchesUnitOfWork
    {
        private readonly IBranchesRepository _repos;

        public BranchesUnitOfWork(IBranchesRepository repos)
        {
            _repos = repos;
        }

        public Task<ActionResponse<Branch>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<Branch>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<Branch>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<IEnumerable<Branch>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<Branch>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<Branch>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);
        
        public Task<ActionResponse<Branch>> AddAsync(Branch model, long Id_Local)=>_repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<Branch>>> AddListAsync(List<Branch> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<Branch>> DeleteAsync(long id, long Id_local)=>_repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<Branch>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<Branch>> DeleteFullAsync(long id)=>_repos.DeleteFullAsync(id);

        public Task<ActionResponse<Branch>> UpdateAsync(Branch model, long Id_Local)=>_repos.UpdateAsync(model, Id_Local);
    }
}
