using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Backend.Helpers;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Backend.UnitsOfWork.Interfaces;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Implementations.Location;
using WMS.Share.DTOs;
using WMS.Backend.UnitsOfWork.Implementations;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace WMS.Backend.Controllers.Location
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class BranchesController : Controller
    {
        private readonly IBranchesUnitOfWork _unitOfWork;
        private readonly IValidateSession _validateSession;
        public BranchesController(IBranchesUnitOfWork unitOfWork,IValidateSession validateSession) 
        {
            _unitOfWork = unitOfWork;
            _validateSession = validateSession;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("genericsearch")]
        public async Task<IActionResult> GetGenericSearchAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.DownloadAsync(pagination);            
            if (response.WasSuccess)
            {
                var List = (List<Branch>)response.Result!;
                var ListReturn = 
                    (from item in List
                    select new GenericSearchDTO
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description
                    }).ToList();
                return Ok(ListReturn);
            }
            return BadRequest();
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetAsync();
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("getasync")]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("downloadasync")]
        public async Task<IActionResult> DownloadAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.DownloadAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetTotalPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _unitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("deletefull")]
        public async Task<IActionResult> GetDeleteAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetDeleteAsync();
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("getdeleteasync")]
        public async Task<IActionResult> GetDeleteAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetDeleteAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("deletetotalPages")]
        public async Task<IActionResult> GetDeleteTotalPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _unitOfWork.GetDeleteTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostAsync(Branch model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Create");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var action = await _unitOfWork.AddAsync(model, user!.Id_Local);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpDelete("deleteasync/{id}")]
        public virtual async Task<IActionResult> DeleteAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Delete");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var response = await _unitOfWork.DeleteAsync(id, user!.Id_Local);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest(response.Message);
        }

        [HttpGet("restoreasync/{id}")]
        public async Task<IActionResult> RestoreAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var response = await _unitOfWork.ActiveAsync(id, user!.Id_Local);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpDelete("deletefullasync/{id}")]
        public virtual async Task<IActionResult> DeleteFullAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Delete");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.DeleteFullAsync(id);
            if (response.WasSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }

        [HttpPut]
        public virtual async Task<IActionResult> PutAsync(Branch model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Update");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var action = await _unitOfWork.UpdateAsync(model, user!.Id_Local);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpPost("uploadasync")]
        public virtual async Task<IActionResult> UploadAsync(List<Branch> list)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Create");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var action = await _unitOfWork.AddListAsync(list, user!.Id_Local);
            return Ok(action);
        }
    }
}
