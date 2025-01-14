using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Location
{
    public interface IStatesUnitOfWork
    {
        Task<ActionResponse<State>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<State>>> GetAsync();

        Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<IEnumerable<State>> GetComboAsync(int countryId);

    }
}
