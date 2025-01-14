using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Location
{
    public interface ICitiesUnitOfWork
    {
        Task<ActionResponse<IEnumerable<City>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<IEnumerable<City>> GetComboAsync(int stateId);
    }
}
