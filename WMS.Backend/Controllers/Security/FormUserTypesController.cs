using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using WMS.Backend.Helpers;
using WMS.Backend.UnitsOfWork.Implementations.Security;
using WMS.Backend.UnitsOfWork.Interfaces;
using WMS.Backend.UnitsOfWork.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;

namespace WMS.Backend.Controllers.Security
{
    [ApiController]

    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FormUserTypesController : GenericController<FormUserType>
    {
        private readonly IGenericUnitOfWork<FormUserType> _unitOfWork;
        private readonly IFormUserTypeUnitOfWork _formuserTypeUnitOfWork;
        private readonly IValidateSession _validateSession;

        public FormUserTypesController(IGenericUnitOfWork<FormUserType> unitOfWork,IFormUserTypeUnitOfWork formuserTypeUnitOfWork,IValidateSession validateSession) : base(unitOfWork, validateSession, 2)
        {
            _unitOfWork = unitOfWork;
            _formuserTypeUnitOfWork = formuserTypeUnitOfWork;
            _validateSession = validateSession;
        }

        [HttpGet("GetFormParentAsync")]
        public  async Task<IActionResult> GetFormParentAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 2, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _formuserTypeUnitOfWork.GetFormParentAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("GetFormParentPagesAsync")]
        public async Task<IActionResult> GetFormParentPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 2, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _formuserTypeUnitOfWork.GetFormParentTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("GetFormAsync")]
        public async Task<IActionResult> GetFormAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 2, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _formuserTypeUnitOfWork.GetFormAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("GetFormPagesAsync")]
        public async Task<IActionResult> GetFormPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 2, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _formuserTypeUnitOfWork.GetFormTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("GetFormUserTypeAsync")]
        public async Task<IActionResult> GetFormUserTypeAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 2, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _formuserTypeUnitOfWork.GetFormUserTypeAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("GetFormUserTypePagesAsync")]
        public async Task<IActionResult> GetFormUserTypePagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 2, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _formuserTypeUnitOfWork.GetFormUserTypeTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("GetFormIdAsync")]
        public async Task<IActionResult> GetFormIdAsync(long Id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 2, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _formuserTypeUnitOfWork.GetFormIdAsync(Id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("GetFormParentIdAsync")]
        public async Task<IActionResult> GetFormParentIdAsync(long Id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 2, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _formuserTypeUnitOfWork.GetFormParentIdAsync(Id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }
}
