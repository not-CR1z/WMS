using Microsoft.EntityFrameworkCore;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Location
{
    public class SubWineriesRepository:ISubWineriesRepository
    {
        private readonly DataContext _context;
        public SubWineriesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ActionResponse<SubWinery>> AddAsync(SubWinery model, long Id_Local)
        {
            var user=await _context.Users.Where(w=>w.Id_Local==Id_Local).FirstOrDefaultAsync();
            if(user==null)
            {
                return new ActionResponse<SubWinery>
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
                return new ActionResponse<SubWinery>
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

                return new ActionResponse<SubWinery>
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

        public async Task<ActionResponse<List<SubWinery>>> AddListAsync(List<SubWinery> list, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<List<SubWinery>>
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
                    var branch = await _context.Branches.AsNoTracking().Where(w => w.Name == item.GenericSearchName1).FirstOrDefaultAsync();
                    if (branch == null)
                    {
                        item.StrError = "No se encuentra sucursal " + item.GenericSearchName1;
                        continue;
                    }
                    var winery = await _context.Wineries.AsNoTracking().Where(w => w.BranchId==branch.Id && w.Name == item.GenericSearchName).FirstOrDefaultAsync();
                    if (winery == null)
                    {
                        item.StrError = "No se encuentra bodega " + item.GenericSearchName;
                        continue;
                    }
                    if (item.Update==false)
                    {

                        item.WineryId = winery.Id;
                        item.CreateUserId = Id_Local;
                        item.CreateDate = DateTime.Now;
                        item.UpdateDate = DateTime.Now;
                        item.UpdateUserId = Id_Local;
                        _context.Add(item);
                        await _context.SaveChangesAsync();
                        
                    }
                    else
                    {
                        var model = await _context.SubWineries.Where(w =>w.Delete==false && w.WineryId==winery.Id && w.Code == item.Code).FirstOrDefaultAsync();
                        if (model == null)
                        {
                            item.StrError = "No se encuentra el registro";
                        }
                        else
                        {
                            model.WineryId = winery.Id;
                            model.Code = item.Code;
                            model.Description = item.Description;
                            model.Active = item.Active;
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
                return new ActionResponse<List<SubWinery>>
                {
                    WasSuccess = false,
                    Message = "Revisar Listado de errores",
                    Result = listerror
                };
            }
            await _context.SaveChangesAsync();
            transaction.Commit();
            return new ActionResponse<List<SubWinery>>
            {
                WasSuccess = true,
                Message = "No se encuentra usuario",
                Result = list,
            };
        }

        public async Task<ActionResponse<SubWinery>> DeleteAsync(long id,long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<SubWinery>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model=await _context.SubWineries.FindAsync(id);
            if(model == null)
            {
                return new ActionResponse<SubWinery>
                {
                    WasSuccess = false,
                    Message="No se encuentra el registro"
                };
            }
            if(model.Delete)
            {
                return new ActionResponse<SubWinery>
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
                return new ActionResponse<SubWinery>
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

                return new ActionResponse<SubWinery>
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

        public async Task<ActionResponse<SubWinery>> ActiveAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<SubWinery>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.SubWineries.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<SubWinery>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (!model.Delete)
            {
                return new ActionResponse<SubWinery>
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
                return new ActionResponse<SubWinery>
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

                return new ActionResponse<SubWinery>
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

        public async Task<ActionResponse<SubWinery>> DeleteFullAsync(long id)
        {
            try
            {
                var model = await _context.SubWineries.FindAsync(id);
                if (model == null)
                {
                    return new ActionResponse<SubWinery>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                if(!model.Delete)
                {
                    return new ActionResponse<SubWinery>
                    {
                        WasSuccess = false,
                        Message = "No se puede eliminar definitivamente hasta que no se elimine previamente"
                    };
                }
                try
                {
                    _context.Remove(model);
                    await _context.SaveChangesAsync();
                    return new ActionResponse<SubWinery>
                    {
                        WasSuccess = true
                    };
                }
                catch
                {
                    return new ActionResponse<SubWinery>
                    {
                        WasSuccess = false,
                        Message = "No se pude borrar, porque tiene registros relacionados."
                    };
                }
            }
            catch (Exception ex)
            {

                return new ActionResponse<SubWinery>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<SubWinery>> GetAsync(long id)
        {
            var model = await _context.SubWineries.Include(I=>I.Winery).ThenInclude(I=>I!.Branch).FirstOrDefaultAsync(w=>w.Id==id);
            if (model == null)
            {
                return new ActionResponse<SubWinery>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            return new ActionResponse<SubWinery>
            {
                WasSuccess = true,
                Result = model
            };
        }

        public async Task<ActionResponse<IEnumerable<SubWinery>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<SubWinery>>
            {
                WasSuccess = true,
                Result = await _context.SubWineries.Where(w=>w.Delete==false).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<SubWinery>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.SubWineries.Where(w => w.Delete == false).Include(I=>I.Winery).ThenInclude(I=>I!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Winery!.Branch!.Id.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.WineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Code.ToString().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<SubWinery>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o=>o.Code).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<SubWinery>>> DownloadAsync(PaginationDTO pagination)
        {
            var queryable = _context.SubWineries.Where(w => w.Delete == false).Include(I => I.Winery).ThenInclude(I => I!.Branch).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Winery!.Branch!.Id.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.WineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Code.ToString().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<SubWinery>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Code).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.SubWineries.Where(w => w.Delete == false).Include(I => I.Winery).ThenInclude(I => I!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Winery!.Branch!.Id.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.WineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Code.ToString().Contains(pagination.Filter.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<SubWinery>>> GetDeleteAsync()
        {
            return new ActionResponse<IEnumerable<SubWinery>>
            {
                WasSuccess = true,
                Result = await _context.SubWineries.Where(w => w.Delete == true).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<SubWinery>>> GetDeleteAsync(PaginationDTO pagination)
        {
            var queryable = _context.SubWineries.Where(w => w.Delete == true).Include(I => I.Winery).ThenInclude(I => I!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Winery!.Branch!.Id.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.WineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Code.ToString().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<SubWinery>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.SubWineries.Where(w => w.Delete == true).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Winery!.Branch!.Id.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.WineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Code.ToString().Contains(pagination.Filter.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<SubWinery>> UpdateAsync(SubWinery model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<SubWinery>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            if (model == null)
            {
                return new ActionResponse<SubWinery>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }  
            
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<SubWinery>
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

                return new ActionResponse<SubWinery>
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

        private ActionResponse<SubWinery> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<SubWinery>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<SubWinery> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<SubWinery>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}
