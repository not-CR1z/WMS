using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Location
{
    public interface ICompanyUnitOfWork
    {
        Task<ActionResponse<Company>> GetAsync();
        Task<ActionResponse<Company>> PostAsync(Company model,long Id_Local);
    }
}
