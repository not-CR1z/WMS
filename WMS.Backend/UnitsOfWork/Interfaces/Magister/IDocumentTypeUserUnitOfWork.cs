using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Interfaces.Magister
{
    public interface IDocumentTypeUserUnitOfWork
    {
        Task<ActionResponse<IEnumerable<DocumentTypeUser>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
    }
}
