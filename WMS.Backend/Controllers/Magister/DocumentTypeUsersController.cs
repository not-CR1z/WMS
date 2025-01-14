using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMS.Backend.UnitsOfWork.Interfaces;
using WMS.Share.DTOs;
using WMS.Backend.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using WMS.Share.Models.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;

namespace WMS.Backend.Controllers.Magister
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DocumentTypeUsersController : GenericController<DocumentTypeUser>
    {
        private readonly IDocumentTypeUserUnitOfWork _documentTypeUserUnitOfWork;
        private readonly IValidateSession _validateSession;

        public DocumentTypeUsersController(IGenericUnitOfWork<DocumentTypeUser> unitOfWork, IDocumentTypeUserUnitOfWork documentTypeUserUnitOfWork, IValidateSession validateSession) : base(unitOfWork, validateSession, 3)
        {
            _documentTypeUserUnitOfWork = documentTypeUserUnitOfWork;
            _validateSession = validateSession;
        }
        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 3, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _documentTypeUserUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 3, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _documentTypeUserUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }
    }
}
