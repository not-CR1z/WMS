using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Location
{
    public interface ICountriesRepository
    {
        Task<ActionResponse<Country>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<Country>>> GetAsync();

        Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<IEnumerable<Country>> GetComboAsync();
    }
}
