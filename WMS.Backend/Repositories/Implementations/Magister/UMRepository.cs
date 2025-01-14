using Microsoft.EntityFrameworkCore;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Magister
{
    public class UMRepository : IUMRepository
    {
        private readonly DataContext _context;
        public UMRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResponse<UM>> GetAsync(long id)
        {
            var model = await _context.UMs.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            return new ActionResponse<UM>
            {
                WasSuccess = true,
                Result = model
            };
        }

        public async Task<ActionResponse<IEnumerable<UM>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<UM>>
            {
                WasSuccess = true,
                Result = await _context.UMs.Where(w => w.Delete == false).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<UM>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.UMs.Where(w => w.Delete == false).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Code.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<UM>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Code).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.UMs.Where(w => w.Delete == false).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Code.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<UM>>> GetDeleteAsync()
        {
            return new ActionResponse<IEnumerable<UM>>
            {
                WasSuccess = true,
                Result = await _context.UMs.Where(w => w.Delete == true).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<UM>>> GetDeleteAsync(PaginationDTO pagination)
        {
            var queryable = _context.UMs.Where(w => w.Delete == true).AsQueryable();
            return new ActionResponse<IEnumerable<UM>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.UMs.Where(w => w.Delete == true).AsQueryable();
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<UM>>> DownloadAsync(PaginationDTO pagination)
        {
            var queryable = _context.UMs.Where(w => w.Delete == false).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Code.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<UM>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Code).ToListAsync()
            };
        }

        public async Task<ActionResponse<UM>> AddAsync(UM model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            try
            {
                model.CreateUserId = Id_Local;
                model.CreateDate = DateTime.Now;
                model.UpdateDate = DateTime.Now;
                model.UpdateUserId = Id_Local;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<UM>
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

                return new ActionResponse<UM>
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

        public async Task<ActionResponse<List<UM>>> AddListAsync(List<UM> list, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<List<UM>>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario",
                    Result = list
                };
            }
            using var transaction = _context.Database.BeginTransaction();

            foreach (var item in list)
            {
                try
                {
                    if (item.Update == false)
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
                        var bintype = await _context.UMs.Where(w => w.Delete == false && w.Code == item.Code).FirstOrDefaultAsync();
                        if (bintype == null)
                        {
                            item.StrError = "No se encuentra el registro";
                        }
                        else
                        {
                            bintype.UpdateUserId = Id_Local;
                            bintype.UpdateDate = DateTime.Now;
                            _context.Update(bintype);
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
            if (listerror.Count > 0)
            {
                transaction.Rollback();
                return new ActionResponse<List<UM>>
                {
                    WasSuccess = false,
                    Message = "Revisar Listado de errores",
                    Result = listerror
                };
            }
            await _context.SaveChangesAsync();
            transaction.Commit();
            return new ActionResponse<List<UM>>
            {
                WasSuccess = true,
                Message = "No se encuentra usuario",
                Result = list,
            };
        }

        public async Task<ActionResponse<UM>> UpdateAsync(UM model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            if (model == null)
            {
                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<UM>
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

                return new ActionResponse<UM>
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

        public async Task<ActionResponse<UM>> ActiveAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.UMs.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (!model.Delete)
            {
                return new ActionResponse<UM>
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
                return new ActionResponse<UM>
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

                return new ActionResponse<UM>
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

        public async Task<ActionResponse<UM>> DeleteAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.UMs.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (model.Delete)
            {
                return new ActionResponse<UM>
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
                return new ActionResponse<UM>
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

                return new ActionResponse<UM>
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

        public async Task<ActionResponse<UM>> DeleteFullAsync(long id)
        {
            try
            {
                var model = await _context.UMs.FindAsync(id);
                if (model == null)
                {
                    return new ActionResponse<UM>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                if (!model.Delete)
                {
                    return new ActionResponse<UM>
                    {
                        WasSuccess = false,
                        Message = "No se puede eliminar definitivamente hasta que no se elimine previamente"
                    };
                }
                try
                {
                    _context.Remove(model);
                    await _context.SaveChangesAsync();
                    return new ActionResponse<UM>
                    {
                        WasSuccess = true
                    };
                }
                catch
                {
                    return new ActionResponse<UM>
                    {
                        WasSuccess = false,
                        Message = "No se pude borrar, porque tiene registros relacionados."
                    };
                }
            }
            catch (Exception ex)
            {

                return new ActionResponse<UM>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        private ActionResponse<UM> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<UM>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<UM> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<UM>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}
