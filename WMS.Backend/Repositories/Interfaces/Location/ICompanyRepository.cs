using WMS.Share.Models.Location;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Location
{
    public interface ICompanyRepository
    {
        Task<ActionResponse<Company>> GetAsync();
        Task<ActionResponse<Company>> PostAsync(Company model,long Id_Local);
    }
}
