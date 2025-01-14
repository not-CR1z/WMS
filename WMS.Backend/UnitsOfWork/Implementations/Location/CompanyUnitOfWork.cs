using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Backend.Repositories.Interfaces;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Share.Models.Location;
using WMS.Share.Models.Security;
using WMS.Share.Responses;
using WMS.Backend.Repositories.Interfaces.Location;

namespace WMS.Backend.UnitsOfWork.Implementations.Location
{
    public class CompanyUnitOfWork : ICompanyUnitOfWork
    {
        private readonly ICompanyRepository _repos;
        public CompanyUnitOfWork(ICompanyRepository repos)
        {
            _repos = repos;
        }
        public Task<ActionResponse<Company>> PostAsync(Company model, long Id_Local)=>_repos.PostAsync(model, Id_Local);

        public Task<ActionResponse<Company>> GetAsync()=>_repos.GetAsync();
    }
}
