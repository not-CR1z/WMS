using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Backend.Helpers;
using WMS.Backend.UnitsOfWork.Implementations.Security;
using WMS.Backend.UnitsOfWork.Interfaces;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;

namespace WMS.Backend.Controllers.Security
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UserTypeController : GenericController<UserType>
    {
        private readonly IUserTypeUnitOfWork _userTypeUnitOfWork;
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IValidateSession _validateSession;

        public UserTypeController(IGenericUnitOfWork<UserType> unitOfWork,IUserTypeUnitOfWork userTypeUnitOfWork,IUsersUnitOfWork usersUnitOfWork,IValidateSession validateSession) : base(unitOfWork, validateSession, 1)
        {
            _userTypeUnitOfWork = userTypeUnitOfWork;
            _usersUnitOfWork = usersUnitOfWork;
            _validateSession = validateSession;
        }
        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 1, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _userTypeUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 1, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _userTypeUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpPost]
        public override async Task<IActionResult> PostAsync(UserType model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 1, "Update");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _userTypeUnitOfWork.PostAsync(model);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }
    }
}
