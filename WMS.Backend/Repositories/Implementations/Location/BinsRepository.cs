using Microsoft.EntityFrameworkCore;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Helpers;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Location
{
    public class BinsRepository:IBinsRepository
    {
        private readonly DataContext _context;
        public BinsRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ActionResponse<Bin>> AddAsync(Bin model, long Id_Local)
        {
            var user=await _context.Users.Where(w=>w.Id_Local==Id_Local).FirstOrDefaultAsync();
            if(user==null)
            {
                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            try
            {
                var subWinery =await  _context.SubWineries.FindAsync(model.SubWineryId);
                if(subWinery==null)
                {
                    return new ActionResponse<Bin>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra Sub-Bodega"
                    };
                }
                var validateBinCode=ValidateBinCode.Validate(model.BinCode);
                if(!validateBinCode.WasSuccess)
                {
                    return new ActionResponse<Bin>
                    {
                        WasSuccess = false,
                        Message = validateBinCode.Message
                    };
                }
                if(validateBinCode.Result!=subWinery.Code)
                {
                    return new ActionResponse<Bin>
                    {
                        WasSuccess = false,
                        Message = "Sub-Bodega no coincide con codigo de ubicación"
                    };
                }

                model.CreateUserId=Id_Local;
                model.CreateDate=DateTime.Now;
                model.UpdateDate=DateTime.Now;
                model.UpdateUserId=Id_Local;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<Bin>
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

                return new ActionResponse<Bin>
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

        public async Task<ActionResponse<List<Bin>>> AddListAsync(List<Bin> list, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<List<Bin>>
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
                    var branch = await _context.Branches.AsNoTracking().Where(w => w.Name == item.GenericSearchName3).FirstOrDefaultAsync();
                    if (branch == null)
                    {
                        item.StrError = "No se encuentra sucursal " + item.GenericSearchName3;
                        continue;
                    }
                    var winery = await _context.Wineries.AsNoTracking().Where(w => w.BranchId==branch.Id && w.Name == item.GenericSearchName2).FirstOrDefaultAsync();
                    if (winery == null)
                    {
                        item.StrError = "No se encuentra bodega " + item.GenericSearchName2;
                        continue;
                    }
                    var subwinery = await _context.SubWineries.AsNoTracking().Where(w => w.WineryId == winery.Id && w.Code.ToString() == item.GenericSearchName1).FirstOrDefaultAsync();
                    if (subwinery == null)
                    {
                        item.StrError = "No se encuentra Sub-bodega " + item.GenericSearchName1;
                        continue;
                    }
                    var binType= await _context.BinTypes.AsNoTracking().Where(w => w.Name == item.GenericSearchName4).FirstOrDefaultAsync();
                    if (binType == null)
                    {
                        item.StrError = "No se encuentra tipo Ubicación " + item.GenericSearchName4;
                        continue;
                    }
                    var validateBinCode = ValidateBinCode.Validate(item.BinCode);
                    if (!validateBinCode.WasSuccess)
                    {
                        item.StrError = validateBinCode.Message;
                        continue;
                    }
                    if (validateBinCode.Result != subwinery.Code)
                    {
                        item.StrError = "Sub-Bodega no coincide con codigo de ubicación";
                        continue;
                    }
                    if (item.Update==false)
                    {

                        item.SubWineryId = subwinery.Id;
                        item.BinTypeId = binType.Id;
                        item.CreateUserId = Id_Local;
                        item.CreateDate = DateTime.Now;
                        item.UpdateDate = DateTime.Now;
                        item.UpdateUserId = Id_Local;
                        _context.Add(item);
                        await _context.SaveChangesAsync();
                        
                    }
                    else
                    {
                        var model = await _context.Bins.Where(w =>w.Delete==false && w.SubWineryId==subwinery.Id && w.BinCode == item.BinCode).FirstOrDefaultAsync();
                        if (model == null)
                        {
                            item.StrError = "No se encuentra el registro";
                        }
                        else
                        {
                            model.SubWineryId = subwinery.Id;
                            model.BinTypeId = binType.Id;
                            model.BinCodeABC = item.BinCodeABC;
                            model.BinCode= item.BinCode;
                            model.BinDescription= item.BinDescription;
                            model.HeightCM = item.HeightCM;
                            model.WidthCM = item.WidthCM;
                            model.DepthCM = item.DepthCM;
                            model.WeightKG = item.WeightKG;
                            model.PercentUsed = item.PercentUsed;
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
                return new ActionResponse<List<Bin>>
                {
                    WasSuccess = false,
                    Message = "Revisar Listado de errores",
                    Result = listerror
                };
            }
            await _context.SaveChangesAsync();
            transaction.Commit();
            return new ActionResponse<List<Bin>>
            {
                WasSuccess = true,
                Message = "No se encuentra usuario",
                Result = list,
            };
        }

        public async Task<ActionResponse<Bin>> DeleteAsync(long id,long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model=await _context.Bins.FindAsync(id);
            if(model == null)
            {
                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message="No se encuentra el registro"
                };
            }
            if(model.Delete)
            {
                return new ActionResponse<Bin>
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
                return new ActionResponse<Bin>
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

                return new ActionResponse<Bin>
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

        public async Task<ActionResponse<Bin>> ActiveAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.Bins.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (!model.Delete)
            {
                return new ActionResponse<Bin>
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
                return new ActionResponse<Bin>
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

                return new ActionResponse<Bin>
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

        public async Task<ActionResponse<Bin>> DeleteFullAsync(long id)
        {
            try
            {
                var model = await _context.Bins.FindAsync(id);
                if (model == null)
                {
                    return new ActionResponse<Bin>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                if(!model.Delete)
                {
                    return new ActionResponse<Bin>
                    {
                        WasSuccess = false,
                        Message = "No se puede eliminar definitivamente hasta que no se elimine previamente"
                    };
                }
                try
                {
                    _context.Remove(model);
                    await _context.SaveChangesAsync();
                    return new ActionResponse<Bin>
                    {
                        WasSuccess = true
                    };
                }
                catch
                {
                    return new ActionResponse<Bin>
                    {
                        WasSuccess = false,
                        Message = "No se pude borrar, porque tiene registros relacionados."
                    };
                }
            }
            catch (Exception ex)
            {

                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<Bin>> GetAsync(long id)
        {
            var model = await _context.Bins.Include(I=>I.BinType).Include(I=>I.SubWinery).ThenInclude(I=>I.Winery).ThenInclude(I=>I!.Branch).FirstOrDefaultAsync(w=>w.Id==id);
            if (model == null)
            {
                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            return new ActionResponse<Bin>
            {
                WasSuccess = true,
                Result = model
            };
        }

        public async Task<ActionResponse<IEnumerable<Bin>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<Bin>>
            {
                WasSuccess = true,
                Result = await _context.Bins.Where(w=>w.Delete==false).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Bin>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Bins.Where(w => w.Delete == false).Include(I => I.BinType).Include(I=>I.SubWinery).ThenInclude(I => I.Winery).ThenInclude(I=>I!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter3))
            {
                queryable = queryable.Where(x => x.SubWinery!.Winery!.Branch!.Id.ToString() == pagination.Filter3);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.SubWinery!.WineryId.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.SubWineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.BinCode.ToString().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Bin>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o=>o.BinCode).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Bin>>> DownloadAsync(PaginationDTO pagination)
        {
            var queryable = _context.Bins.Where(w => w.Delete == false).Include(I => I.BinType).Include(I => I.SubWinery).ThenInclude(I => I.Winery).ThenInclude(I => I!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter3))
            {
                queryable = queryable.Where(x => x.SubWinery!.Winery!.Branch!.Id.ToString() == pagination.Filter3);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.SubWinery!.WineryId.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.SubWineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.BinCode.ToString().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Bin>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.BinCode).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Bins.Where(w => w.Delete == false).Include(I => I.BinType).Include(I => I.SubWinery).ThenInclude(I => I.Winery).ThenInclude(I => I!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter3))
            {
                queryable = queryable.Where(x => x.SubWinery!.Winery!.Branch!.Id.ToString() == pagination.Filter3);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.SubWinery!.WineryId.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.SubWineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.BinCode.ToString().Contains(pagination.Filter.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<Bin>>> GetDeleteAsync()
        {
            return new ActionResponse<IEnumerable<Bin>>
            {
                WasSuccess = true,
                Result = await _context.Bins.Where(w => w.Delete == true).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Bin>>> GetDeleteAsync(PaginationDTO pagination)
        {
            var queryable = _context.Bins.Where(w => w.Delete == true).Include(I => I.BinType).Include(I => I.SubWinery).ThenInclude(I => I.Winery).ThenInclude(I => I!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter3))
            {
                queryable = queryable.Where(x => x.SubWinery!.Winery!.Branch!.Id.ToString() == pagination.Filter3);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.SubWinery!.WineryId.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.SubWineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.BinCode.ToString().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Bin>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Bins.Where(w => w.Delete == true).Include(I => I.BinType).Include(I => I.SubWinery).ThenInclude(I => I.Winery).ThenInclude(I => I!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter3))
            {
                queryable = queryable.Where(x => x.SubWinery!.Winery!.Branch!.Id.ToString() == pagination.Filter3);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.SubWinery!.WineryId.ToString() == pagination.Filter2);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.SubWineryId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.BinCode.ToString().Contains(pagination.Filter.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<Bin>> UpdateAsync(Bin model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            if (model == null)
            {
                return new ActionResponse<Bin>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }  
            
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<Bin>
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

                return new ActionResponse<Bin>
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

        private ActionResponse<Bin> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<Bin>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<Bin> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<Bin>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}
