using WMS.Share.DTOs;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces
{
    public interface IGenericUnitOfWork<T> where T : class
    {
        Task<ActionResponse<IEnumerable<T>>> GetAsync();

        Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);


        Task<ActionResponse<T>> AddAsync(T model);

        Task<ActionResponse<T>> UpdateAsync(T model);

        Task<ActionResponse<T>> DeleteAsync(long id);

        Task<ActionResponse<T>> GetAsync(long id);
    }
}
