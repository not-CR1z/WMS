using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Backend.UnitsOfWork.Implementations.Security;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Helpers;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;

namespace WMS.Backend.Controllers.Magister
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IGenericRepository<UserType> _genericRepository;
        private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly IFormUserTypeUnitOfWork _formUserTypeUnitOfWork;
        private readonly IValidateSession _validateSession;
        private readonly string _container;

        public UsersController(IUsersUnitOfWork usersUnitOfWork, IGenericRepository<UserType> genericRepository, IConfiguration configuration, IFileStorage fileStorage, IFormUserTypeUnitOfWork formUserTypeUnitOfWork, IValidateSession validateSession)
        {
            _usersUnitOfWork = usersUnitOfWork;
            _genericRepository = genericRepository;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _formUserTypeUnitOfWork = formUserTypeUnitOfWork;
            _validateSession = validateSession;
            _container = "users";
        }

        [HttpPost("CreateUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Create");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            if (model.DocumentTypeUserId == 0)
            {
                return BadRequest("Tipo de Documento Obligatorio");
            }
            var userdb = await _usersUnitOfWork.GetUserAsync(model.Email!);
            if (userdb != null)
            {
                return BadRequest("Email ya esta en uso");
            }
            User user = model;
            if (!string.IsNullOrEmpty(user.Photo))
            {
                var photoUser = Convert.FromBase64String(user.Photo);
                user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
            }
            var result = await _usersUnitOfWork.AddUserAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault());
            }
            if (model.UserTyepIds!.Count > 0)
            {
                await _usersUnitOfWork.UserToRoleAsync(user, model.UserTyepIds);
            }

            return Ok(user);
        }

        [HttpPost("ChangeEmailPassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangeEmailPassword(ChangePasswordDTO model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Update");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var userdb = await _usersUnitOfWork.GetIdLocalAsync(model.Id_Local!);
            if (userdb.Email != model.Email)
            {
                var useremail = await _usersUnitOfWork.GetUserAsync(model.Email!);
                if (useremail != null)
                {
                    return BadRequest("Email ya esta en uso");
                }
                var result = await _usersUnitOfWork.ChangeEmailAsync(userdb, model.Email);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.FirstOrDefault());
                }
                userdb.Email = model.Email;
                var result0 = await _usersUnitOfWork.UpdateUserAsync(userdb);
                if (!result0.Succeeded)
                {
                    return BadRequest(result.Errors.FirstOrDefault());
                }
            }
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var result1 = await _usersUnitOfWork.ResetPasswordAsync(userdb, model.NewPassword);
                if (!result1.Succeeded)
                {
                    return BadRequest(result1.Errors.FirstOrDefault());
                }
            }
            return Ok(userdb);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser(User model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Update");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var currentUser = await _usersUnitOfWork.GetAsync(model.Id!);
            if (currentUser == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(model.Photo))
            {
                var photoUser = Convert.FromBase64String(model.Photo);
                model.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
            }
            User user = model;
            currentUser.Document = user.Document;
            currentUser.FirstName = user.FirstName;
            currentUser.LastName = user.LastName;
            currentUser.Address = user.Address;
            currentUser.PhoneNumber = user.PhoneNumber;
            currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
            var result = await _usersUnitOfWork.UpdateUserAsync(currentUser);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault());
            }

            var response2 = await _usersUnitOfWork.UserToRoleAsync(currentUser, model.UserTyepIds!);
            if (!response2.WasSuccess)
            {
                return BadRequest(response2.Message);
            }

            currentUser.UserTyepIds = model.UserTyepIds;
            return Ok(currentUser);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
        {
            var result = await _usersUnitOfWork.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _usersUnitOfWork.GetUserAsync(model.Email);
                var userTypes = await _usersUnitOfWork.GetRoleUserAsync(user.Id_Local);
                var token = await BuildTokenAsync(user, userTypes);
                if (!token.WasSuccess)
                {
                    return BadRequest(token.Message);
                }
                return Ok(token);
            }

            return BadRequest("Email o contraseña incorrectos.");
        }

        private async Task<TokenDTO> BuildTokenAsync(User user, List<UserType> userTypes)
        {
            try
            {
                List<FormParentDTO> formUserTypeList = [];
                string jsonRoles = string.Empty;
                if (userTypes != null && userTypes.Count > 0)
                {
                    jsonRoles = JsonConvert.SerializeObject(userTypes);

                }
                var responseFormParent = await _formUserTypeUnitOfWork.GetFormParentUser(user.Id_Local);
                if (responseFormParent.WasSuccess)
                {
                    formUserTypeList = responseFormParent.Result!.ToList();
                }
                string jsonForms = string.Empty;
                if (formUserTypeList.Count > 0)
                {
                    jsonForms = JsonConvert.SerializeObject(formUserTypeList);
                }
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Email!),
                    //new("Roles", rolesjson
                    new("Id_Local",user.Id_Local.ToString()),
                    new(ClaimTypes.Role, jsonRoles),
                    new("Forms", jsonForms),
                    new("Document", user.Document),
                    new("FirstName", user.FirstName),
                    new("LastName", user.LastName),
                    new("Address", user.Address),
                    new("Photo", user.Photo ?? string.Empty),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtkey"]!));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiration = DateTime.UtcNow.AddHours(24);
                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: credentials);

                return new TokenDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expiration,
                    WasSuccess = true
                };
            }
            catch (Exception ex)
            {

                return new TokenDTO
                {
                    Message = ex.Message,
                    WasSuccess = false
                };
            }
        }

        [HttpGet("GetAsync/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAsync(string Id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _usersUnitOfWork.GetAsync(Id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet("GetIdLocalAsync/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetIdLocalAsync(long Id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _usersUnitOfWork.GetIdLocalAsync(Id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet("GetRoleUserAsync/{Id_Local}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetRoleUserAsync(long Id_Local)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _usersUnitOfWork.GetRoleUserAsync(Id_Local);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet("GetAsync")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _usersUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _usersUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<IActionResult> DeleteAsync(long Id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 4, "Delete");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = await _usersUnitOfWork.GetIdLocalAsync(Id);
            if (user == null)
            {
                return BadRequest("No se encuentra usuario");
            }
            var action = await _usersUnitOfWork.DeleteUserAsync(user);
            if (action.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(action.Errors);
        }

        [HttpPost("Clock")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ClockAsync(UserUpdateDTO model)
        {
            if (model.CreateUserId != 0)
            {
                model.CreateUser = await _usersUnitOfWork.GetIdLocalAsync(model.CreateUserId);
            }


            if (model.UpdateUserId != 0)
            {
                model.UpdateUser = await _usersUnitOfWork.GetIdLocalAsync(model.UpdateUserId);
            }


            if (model.ChangeStateUserId != 0)
            {
                model.ChangeStateUser = await _usersUnitOfWork.GetIdLocalAsync(model.ChangeStateUserId);
            }


            if (model.DeleteUserId != 0)
            {
                model.DeleteUser = await _usersUnitOfWork.GetIdLocalAsync(model.DeleteUserId);
            }

            return Ok(model);
            
        }

    }
}
