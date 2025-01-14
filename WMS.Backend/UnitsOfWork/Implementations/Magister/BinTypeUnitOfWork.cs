using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class BinTypeUnitOfWork : IBinTypeUnitOfWork
    {
        private readonly IBinTypeRepository _repos;

        public BinTypeUnitOfWork(IBinTypeRepository repos)
        {
            _repos = repos;
        }

        public Task<ActionResponse<BinType>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<BinType>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<BinType>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<BinType>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<BinType>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<BinType>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<BinType>> AddAsync(BinType model, long Id_Local)=>_repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<BinType>>> AddListAsync(List<BinType> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<BinType>> UpdateAsync(BinType model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);

        public Task<ActionResponse<BinType>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<BinType>> DeleteAsync(long id, long Id_local)=>_repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<BinType>> DeleteFullAsync(long id)=>_repos.DeleteFullAsync(id);


    }
}
