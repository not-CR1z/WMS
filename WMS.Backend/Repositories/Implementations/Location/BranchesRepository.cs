using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NPOI.HSSF.Record.Aggregates;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Location
{
    public class BranchesRepository : IBranchesRepository
    {
        private readonly DataContext _context;
        public BranchesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ActionResponse<Branch>> AddAsync(Branch model, long Id_Local)
        {
            var user=await _context.Users.Where(w=>w.Id_Local==Id_Local).FirstOrDefaultAsync();
            if(user==null)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            try
            {
                model.CreateUserId=Id_Local;
                model.CreateDate=DateTime.Now;
                model.UpdateDate=DateTime.Now;
                model.UpdateUserId=Id_Local;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<Branch>
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

                return new ActionResponse<Branch>
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

        public async Task<ActionResponse<List<Branch>>> AddListAsync(List<Branch> list, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<List<Branch>>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario",
                    Result=list
                };
            }
            using var transaction = _context.Database.BeginTransaction();
            
            foreach (var item in list)
            {
                try
                {
                    if(item.Update==false)
                    {
                        item.CreateUserId = Id_Local;
                        item.CreateDate = DateTime.Now;
                        item.UpdateDate = DateTime.Now;
                        item.UpdateUserId = Id_Local;
                        _context.Add(item);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var model = await _context.Branches.Where(w =>w.Delete==false && w.Name == item.Name).FirstOrDefaultAsync();
                        if (model == null)
                        {
                            item.StrError = "No se encuentra el registro";
                        }
                        else
                        {
                            model.Name = item.Name;
                            model.Description = item.Description;
                            model.Contact = item.Contact;
                            model.PhoneContact = item.PhoneContact;
                            model.EmailContact = item.EmailContact;
                            model.Contingency = item.Contingency;
                            model.EmailFromNotification = item.EmailFromNotification;
                            model.EmailFromNotificationPassword = item.EmailFromNotificationPassword;
                            model.EmailFromHost = item.EmailFromHost;
                            model.EmailFromPort = item.EmailFromPort;
                            model.EmailFromSsl = item.EmailFromSsl;
                            model.UpdateUserId= Id_Local;
                            model.UpdateDate= DateTime.Now;
                            _context.Update(model);
                            await _context.SaveChangesAsync();
                        }

                    }

                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException!.Message.Contains("duplicate"))
                        {
                            item.StrError = "Ya existe este registro o está en eliminados";
                        }
                        else
                        {
                            item.StrError = ex.Message;
                        }
                    }
                    else
                    {
                        item.StrError = ex.Message;
                    }

                }
                catch (Exception ex)
                {
                    item.StrError = ex.Message;
                }
            }
            var listerror = list.Where(w => w.StrError != null).ToList();
            if(listerror.Count > 0)
            {
                transaction.Rollback();
                return new ActionResponse<List<Branch>>
                {
                    WasSuccess = false,
                    Message = "Revisar Listado de errores",
                    Result = listerror
                };
            }
            await _context.SaveChangesAsync();
            transaction.Commit();
            return new ActionResponse<List<Branch>>
            {
                WasSuccess = true,
                Message = "No se encuentra usuario",
                Result = list,
            };
        }

        public async Task<ActionResponse<Branch>> DeleteAsync(long id,long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model=await _context.Branches.FindAsync(id);
            if(model == null)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message="No se encuentra el registro"
                };
            }
            if(model.Delete)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "El registro ya se encuentra eliminado"
                };
            }
            model.DeleteUserId = Id_Local;
            model.DeleteDate = DateTime.Now;
            model.Delete = true;
            _context.Update(model);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Branch>
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

                return new ActionResponse<Branch>
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

        public async Task<ActionResponse<Branch>> ActiveAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.Branches.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (!model.Delete)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "El registro ya se encuentra activo"
                };
            }
            model.UpdateUserId = Id_Local;
            model.UpdateDate = DateTime.Now;
            model.Delete = false;
            _context.Update(model);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Branch>
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

                return new ActionResponse<Branch>
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

        public async Task<ActionResponse<Branch>> DeleteFullAsync(long id)
        {
            try
            {
                var model = await _context.Branches.FindAsync(id);
                if (model == null)
                {
                    return new ActionResponse<Branch>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                if(!model.Delete)
                {
                    return new ActionResponse<Branch>
                    {
                        WasSuccess = false,
                        Message = "No se puede eliminar definitivamente hasta que no se elimine previamente"
                    };
                }
                try
                {
                    _context.Remove(model);
                    await _context.SaveChangesAsync();
                    return new ActionResponse<Branch>
                    {
                        WasSuccess = true
                    };
                }
                catch
                {
                    return new ActionResponse<Branch>
                    {
                        WasSuccess = false,
                        Message = "No se pude borrar, porque tiene registros relacionados."
                    };
                }
            }
            catch (Exception ex)
            {

                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<Branch>> GetAsync(long id)
        {
            var model = await _context.Branches.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            return new ActionResponse<Branch>
            {
                WasSuccess = true,
                Result = model
            };
        }

        public async Task<ActionResponse<IEnumerable<Branch>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<Branch>>
            {
                WasSuccess = true,
                Result = await _context.Branches.Where(w=>w.Delete==false).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Branch>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Branches.Where(w => w.Delete == false).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Branch>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o=>o.Name).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Branch>>> DownloadAsync(PaginationDTO pagination)
        {
            var queryable = _context.Branches.Where(w => w.Delete == false).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Branch>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Name).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Branches.Where(w => w.Delete == false).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<Branch>>> GetDeleteAsync()
        {
            return new ActionResponse<IEnumerable<Branch>>
            {
                WasSuccess = true,
                Result = await _context.Branches.Where(w => w.Delete == true).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Branch>>> GetDeleteAsync(PaginationDTO pagination)
        {
            var queryable = _context.Branches.Where(w => w.Delete == true).AsQueryable();
            return new ActionResponse<IEnumerable<Branch>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Branches.Where(w => w.Delete == true).AsQueryable();
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<Branch>> UpdateAsync(Branch model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            if (model == null)
            {
                return new ActionResponse<Branch>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }  
            
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<Branch>
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

                return new ActionResponse<Branch>
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

        private ActionResponse<Branch> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<Branch>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<Branch> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<Branch>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }

    }
}
