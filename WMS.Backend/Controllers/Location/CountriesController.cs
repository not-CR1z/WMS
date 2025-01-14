using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.UnitsOfWork.Interfaces;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;

namespace WMS.Backend.Controllers.Location
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class CountriesController : GenericController<Country>
    {
        private readonly ICountriesUnitOfWork _countriesUnitOfWork;
        private readonly IValidateSession _validateSession;

        public CountriesController(IGenericUnitOfWork<Country> unitOfWork, ICountriesUnitOfWork countriesUnitOfWork, IValidateSession validateSession) : base(unitOfWork, validateSession, 5)
        {
            _countriesUnitOfWork = countriesUnitOfWork;
            _validateSession = validateSession;
        }

        [HttpGet("full")]
        public override async Task<IActionResult> GetAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 5, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _countriesUnitOfWork.GetAsync();
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }


        [HttpGet]
        public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 5, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _countriesUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 6, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _countriesUnitOfWork.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return NotFound(response.Message);
        }

        [HttpGet("totalPages")]
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 5, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _countriesUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        //[AllowAnonymous]
        [HttpGet("combo")]
        public async Task<IActionResult> GetComboAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 5, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            return Ok(await _countriesUnitOfWork.GetComboAsync());
        }
    }
}
