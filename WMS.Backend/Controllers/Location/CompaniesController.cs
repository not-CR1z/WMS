using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Backend.Helpers;
using WMS.Backend.UnitsOfWork.Interfaces;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Security;

namespace WMS.Backend.Controllers.Location
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class CompaniesController:Controller
    {
        private readonly ICompanyUnitOfWork _companyunitOfWork;
        private readonly IValidateSession _validateSession;
        private readonly IFileStorage _fileStorage;
        private readonly string _container;

        public CompaniesController(ICompanyUnitOfWork companyunitOfWork, IValidateSession validateSession, IFileStorage fileStorage) 
        {
            _companyunitOfWork = companyunitOfWork;
            _validateSession = validateSession;
            _fileStorage = fileStorage;
            _container = "company";
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 6, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _companyunitOfWork.GetAsync();
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Company model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 6, "Update");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result!;
            if (!string.IsNullOrEmpty(model.Logo))
            {
                var logoCompany = Convert.FromBase64String(model.Logo);
                model.Logo = await _fileStorage.SaveFileAsync(logoCompany, ".jpg", _container);
            }
            Company currentCompany=new();
            if (model.Id != 0)
            {
                var response = await _companyunitOfWork.GetAsync();
                currentCompany = response.Result!;
                if (model.Id != currentCompany!.Id)
                {
                    return BadRequest("El registro ya no existe");
                }
                currentCompany.Name = model.Name;
                currentCompany.Description = model.Description;
                currentCompany.CreateDate=model.CreateDate;
                currentCompany.Email = model.Email;
                currentCompany.CreateUserId = model.CreateUserId;
                currentCompany.CityId = model.CityId;
                currentCompany.Licence=model.Licence;
                currentCompany.StarLicence=model.StarLicence;
                currentCompany.EndLicence=model.EndLicence;
                currentCompany.Logo= !string.IsNullOrEmpty(model.Logo) && model.Logo != currentCompany.Logo ? model.Logo : currentCompany.Logo;

            }
            else
            {
                currentCompany = model;
            }
            var action = await _companyunitOfWork.PostAsync(currentCompany, user.Id_Local);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }
    }
}
