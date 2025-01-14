using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Backend.Repositories.Interfaces;
using WMS.Share.DTOs;
using WMS.Share.Responses;
using WMS.Share.Models.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Backend.Repositories.Interfaces.Magister;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class DocumentTypeUserUnitOfWork : GenericUnitOfWork<DocumentTypeUser>, IDocumentTypeUserUnitOfWork
    {
        private readonly IDocumentTypeUserRepository _repos;

        public DocumentTypeUserUnitOfWork(IGenericRepository<DocumentTypeUser> repository, IDocumentTypeUserRepository repos) : base(repository)
        {
            _repos = repos;
        }
        public override async Task<ActionResponse<IEnumerable<DocumentTypeUser>>> GetAsync(PaginationDTO pagination) => await _repos.GetAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => await _repos.GetTotalPagesAsync(pagination);
    }
}
