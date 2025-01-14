using WMS.Backend.Repositories.Interfaces;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Backend.UnitsOfWork.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Security
{
    public class UserTypeUnitOfWork : GenericUnitOfWork<UserType>, IUserTypeUnitOfWork
    {
        private readonly IUserTypeRepository _repos;

        public UserTypeUnitOfWork(IGenericRepository<UserType> repository,IUserTypeRepository repos) : base(repository)
        {
            _repos = repos;
        }

        public async Task<ActionResponse<UserType>> PostAsync(UserType Model)=> await _repos.PostAsync(Model);
        public override async Task<ActionResponse<IEnumerable<UserType>>> GetAsync(PaginationDTO pagination) => await _repos.GetAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => await _repos.GetTotalPagesAsync(pagination);

    }
}
