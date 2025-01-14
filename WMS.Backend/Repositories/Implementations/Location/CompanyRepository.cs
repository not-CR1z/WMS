using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WMS.Backend.Data;
using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Location
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DataContext _context;
        public CompanyRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ActionResponse<Company>> PostAsync(Company model, long Id_Local)
        {
            if (Id_Local == 0)
            {
                return new ActionResponse<Company>
                {
                    WasSuccess = false,
                    Message = "Usuario Invalido"
                };
            }
            if (model.Id!=0)
            {
                try
                {
                    return await UpdateAsync(model, Id_Local);
                }
                catch (Exception ex)
                {
                    return new ActionResponse<Company>
                    {
                        WasSuccess = false,
                        Message = ex.Message
                    };
                }
                
            }
            model.CreateDate= DateTime.Now;
            model.CreateUserId = Id_Local;
            model.UpdateDate= DateTime.Now;
            model.UpdateUserId = Id_Local;            
            await _context.AddAsync(model);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Company>
                {
                    WasSuccess = true,
                    Result = model
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponse<Company>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }
        }

        public async Task<ActionResponse<Company>> GetAsync()
        {
            var company=await _context.Companies.Include(I=>I.City).ThenInclude(I=>I!.State).ThenInclude(I=>I!.Country).FirstOrDefaultAsync();
            if(company == null)
            {
                company = new Company
                {
                    StarLicence=DateTime.Now,
                    EndLicence=DateTime.Now,
                };
            }
            return new ActionResponse<Company>
            {
                WasSuccess = true,
                Result = company
            };
        }

        public async Task<ActionResponse<Company>> UpdateAsync(Company model,long Id_Local)
        {
            model.UpdateDate = DateTime.Now;
            model.UpdateUserId = Id_Local;
            
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<Company>
                {
                    WasSuccess = true,
                    Result = model
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponse<Company>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }
        }

        private ActionResponse<Company> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<Company>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<Company> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<Company>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}
