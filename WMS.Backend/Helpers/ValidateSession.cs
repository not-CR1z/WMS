using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using WMS.Backend.Data;
using WMS.Backend.UnitsOfWork.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Helpers
{
    public class ValidateSession : IValidateSession
    {
        private readonly DataContext _context;

        public ValidateSession(DataContext context)
        {
            _context = context;
        }
        public async Task<ActionResponse<User>> GetValidateSession(HttpContext httpContext, int FormCode, string Action)
        {
			try
			{
                ClaimsIdentity? claimsIdentity = httpContext.User.Identity as ClaimsIdentity;
                if (claimsIdentity == null)
                {
                    return new ActionResponse<User>
                    {
                        Message = "No esta Logueado",
                        WasSuccess = false,
                    };
                }                
                long Id_Local = Convert.ToInt64(claimsIdentity!.FindAll("Id_Local").FirstOrDefault()!.Value);
                var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new ActionResponse<User>
                    {
                        WasSuccess = false,
                        Message = "Usuario no encontrado",
                        Result=user
                    };
                }
                if (user.SuperU)
                {
                    return new ActionResponse<User>
                    {
                        WasSuccess = true,
                        Result=user,
                    };
                }
                string jsonForms = claimsIdentity!.FindAll("Forms").FirstOrDefault()!.Value;
                if (jsonForms == null)
                {
                    return new ActionResponse<User>
                    {
                        Message = "No esta Autorizado",
                        WasSuccess = false,
                    };
                }
                var form = _context.Forms.Where(w => w.FormCode == FormCode).FirstOrDefault();
                if(form == null)
                {
                    return new ActionResponse<User>
                    {
                        WasSuccess = false,
                        Message = "Formulario no encontrado",
                        Result = user
                    };
                }
                var userTypeUser = await _context.UserTypeUsers.Where(w => w.UserIdLocal == user.Id_Local).ToListAsync();
                if(userTypeUser == null)
                {
                    return new ActionResponse<User>
                    {
                        WasSuccess = false,
                        Message = "Usuario no tiene roles asignados",
                        Result = user
                    };
                }
                var formUserType= (from item in _context.FormUserTypes.Where(w => w.FormId == form!.Id)
                                  join item2 in userTypeUser on item.UserTypeId equals item2.UserTypeId
                                  select item).FirstOrDefault();
                if (formUserType == null)
                {
                    return new ActionResponse<User>
                    {
                        WasSuccess = false,
                        Message = "Usuario no tiene permisos para este formulario",
                        Result = user
                    };
                }
                switch (Action)
                {
                    case "Create":
                        if (formUserType.Create)
                        {
                            return new ActionResponse<User>
                            {
                                WasSuccess = true,
                                Result = user,
                            };
                        }
                        break;
                    case "Read":
                        if (formUserType.Read)
                        {
                            return new ActionResponse<User>
                            {
                                WasSuccess = true,
                                Result = user,
                            };
                        }
                        break;
                    case "Update":
                        if (formUserType.Update)
                        {
                            return new ActionResponse<User>
                            {
                                WasSuccess = true,
                                Result = user,
                            };
                        }
                        break;
                    case "Delete":
                        if (formUserType.Delete)
                        {
                            return new ActionResponse<User>
                            {
                                WasSuccess = true,
                                Result = user,
                            };
                        }
                        break;
                }

                        //var formParents=  JsonConvert.DeserializeObject<List<FormParentDTO>>(jsonForms)!;
                        //switch (Action)
                        //{
                        //    case "Create":

                        //        foreach (var item in formParents)
                        //        {
                        //            var form = item.FormSubParentDTOs.Forms!.Where(w => w.FormCode == FormCode && w.Create == true);
                        //            if (form.Count() > 0)
                        //            {
                        //                return new ActionResponse<User>
                        //                {
                        //                    WasSuccess = true,
                        //                    Result=user,
                        //                };
                        //            }
                        //        }
                        //        break;
                        //    case "Read":
                        //        foreach (var item in formParents)
                        //        {
                        //            var form = item.Forms!.Where(w => w.FormCode == FormCode && w.Read == true);
                        //            if (form.Count() > 0)
                        //            {
                        //                return new ActionResponse<User>
                        //                {
                        //                    WasSuccess = true,
                        //                    Result=user,
                        //                };
                        //            }
                        //        }
                        //        break;
                        //    case "Update":
                        //        foreach (var item in formParents)
                        //        {
                        //            var form = item.Forms!.Where(w => w.FormCode == FormCode && w.Update == true);
                        //            if (form.Count() > 0)
                        //            {
                        //                return new ActionResponse<User>
                        //                {
                        //                    WasSuccess = true,
                        //                    Result = user,
                        //                };
                        //            }
                        //        }
                        //        break;
                        //    case "Delete":
                        //        foreach (var item in formParents)
                        //        {
                        //            var form = item.Forms!.Where(w => w.FormCode == FormCode && w.Delete == true);
                        //            if (form.Count() > 0)
                        //            {
                        //                return new ActionResponse<User>
                        //                {
                        //                    WasSuccess = true,
                        //                    Result = user,
                        //                };
                        //            }
                        //        }
                        //        break;
                        //}


                return new ActionResponse<User>
                {
                    WasSuccess = false,
                    Message="No Autorizado"
                };

            }
			catch (Exception ex)
			{

                return new ActionResponse<User>
                {
                    Message = ex.Message,
                    WasSuccess = false,
                };
			}
        }
    }
}
