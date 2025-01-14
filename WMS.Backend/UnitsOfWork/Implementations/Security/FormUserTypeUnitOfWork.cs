using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Backend.Repositories.Interfaces;
using WMS.Backend.UnitsOfWork.Interfaces.Security;
using WMS.Share.Models.Security;
using WMS.Share.Responses;
using WMS.Share.DTOs;

namespace WMS.Backend.UnitsOfWork.Implementations.Security
{
    public class FormUserTypeUnitOfWork : GenericUnitOfWork<FormUserType>, IFormUserTypeUnitOfWork
    {
        private readonly IFormUserTypeRepository _repos;

        public async Task<ActionResponse<FormParent>> GetFormParentIdAsync(long Id) => await _repos.GetFormParentIdAsync(Id);
        public async Task<ActionResponse<Form>> GetFormIdAsync(long Id)=> await _repos.GetFormIdAsync(Id);

        public FormUserTypeUnitOfWork(IGenericRepository<FormUserType> repository, IFormUserTypeRepository repos) : base(repository)
        {
            _repos = repos;
        }

        public async Task<ActionResponse<IEnumerable<FormParent>>> GetFormParentAsync(PaginationDTO pagination) => await _repos.GetFormParentAsync(pagination);

        public async Task<ActionResponse<int>> GetFormParentTotalPagesAsync(PaginationDTO pagination) => await _repos.GetFormParentTotalPagesAsync(pagination);

        public async Task<ActionResponse<IEnumerable<Form>>> GetFormAsync(PaginationDTO pagination)=> await _repos.GetFormAsync(pagination);

        public async Task<ActionResponse<int>> GetFormTotalPagesAsync(PaginationDTO pagination)=>await _repos.GetFormTotalPagesAsync(pagination);

        public async Task<ActionResponse<IEnumerable<FormUserType>>> GetFormUserTypeAsync(PaginationDTO pagination) => await _repos.GetFormUserTypeAsync(pagination);

        public async Task<ActionResponse<int>> GetFormUserTypeTotalPagesAsync(PaginationDTO pagination) => await _repos.GetFormUserTypeTotalPagesAsync(pagination);

        public async Task<ActionResponse<List<FormParentDTO>>> GetFormParentUser(long Id_local) => await _repos.GetFormParentUser(Id_local);

    }
}
