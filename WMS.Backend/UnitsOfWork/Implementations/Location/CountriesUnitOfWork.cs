using WMS.Backend.Repositories.Interfaces;
using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations
{
    public class CountriesUnitOfWork : GenericUnitOfWork<Country>, ICountriesUnitOfWork
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesUnitOfWork(IGenericRepository<Country> repository, ICountriesRepository countriesRepository) : base(repository)
        {
            _countriesRepository = countriesRepository;
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => await _countriesRepository.GetTotalPagesAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination) => await _countriesRepository.GetAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync() => await _countriesRepository.GetAsync();

        public override async Task<ActionResponse<Country>> GetAsync(long id) => await _countriesRepository.GetAsync(id);

        public async Task<IEnumerable<Country>> GetComboAsync() => await _countriesRepository.GetComboAsync();
    }
}
