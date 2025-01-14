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
    public class ProductClassificationRepository : IProductClassificationRepository
    {
        private readonly DataContext _context;
        public ProductClassificationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResponse<ProductClassification>> GetAsync(long id)
        {
            var model = await _context.ProductClassifications.Include(I => I.ProductClassificationDetails!.Where(w => w.Delete == false)).FirstOrDefaultAsync(w=>w.Id==id);
            if (model == null)
            {
                return new ActionResponse<ProductClassification>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            return new ActionResponse<ProductClassification>
            {
                WasSuccess = true,
                Result = model
            };
        }

        public async Task<ActionResponse<IEnumerable<ProductClassification>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<ProductClassification>>
            {
                WasSuccess = true,
                Result = await _context.ProductClassifications.Where(w => w.Delete == false).Include(I=>I.ProductClassificationDetails!.Where(w => w.Delete == false)).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<ProductClassification>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.ProductClassifications.Where(w => w.Delete == false).Include(I => I.ProductClassificationDetails!.Where(w=>w.Delete==false)).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<ProductClassification>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Name).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.ProductClassifications.Where(w => w.Delete == false).AsQueryable();
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

        public async Task<ActionResponse<IEnumerable<ProductClassification>>> GetDeleteAsync()
        {
            return new ActionResponse<IEnumerable<ProductClassification>>
            {
                WasSuccess = true,
                Result = await _context.ProductClassifications.Where(w => w.Delete == true).Include(I => I.ProductClassificationDetails!.Where(w => w.Delete == false)).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<ProductClassification>>> GetDeleteAsync(PaginationDTO pagination)
        {
            var queryable = _context.ProductClassifications.Where(w => w.Delete == true).Include(I => I.ProductClassificationDetails!.Where(w => w.Delete == false)).AsQueryable();
            return new ActionResponse<IEnumerable<ProductClassification>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.ProductClassifications.Where(w => w.Delete == true).AsQueryable();
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<ProductClassification>>> DownloadAsync(PaginationDTO pagination)
        {
            var queryable = _context.ProductClassifications.Where(w => w.Delete == false).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<ProductClassification>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Name).ToListAsync()
            };
        }

        public async Task<ActionResponse<ProductClassification>> AddAsync(ProductClassification model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<ProductClassification>
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
                return new ActionResponse<ProductClassification>
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

                return new ActionResponse<ProductClassification>
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

        public async Task<ActionResponse<List<ProductClassification>>> AddListAsync(List<ProductClassification> list, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<List<ProductClassification>>
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
                        var bintype = await _context.ProductClassifications.Where(w => w.Delete == false && w.Name == item.Name).FirstOrDefaultAsync();
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
                return new ActionResponse<List<ProductClassification>>
                {
                    WasSuccess = false,
                    Message = "Revisar Listado de errores",
                    Result = listerror
                };
            }
            await _context.SaveChangesAsync();
            transaction.Commit();
            return new ActionResponse<List<ProductClassification>>
            {
                WasSuccess = true,
                Message = "No se encuentra usuario",
                Result = list,
            };
        }

        public async Task<ActionResponse<ProductClassification>> UpdateAsync(ProductClassification model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<ProductClassification>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            if (model == null)
            {
                return new ActionResponse<ProductClassification>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<ProductClassification>
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

                return new ActionResponse<ProductClassification>
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

        public async Task<ActionResponse<ProductClassification>> ActiveAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<ProductClassification>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.ProductClassifications.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<ProductClassification>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (!model.Delete)
            {
                return new ActionResponse<ProductClassification>
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
                return new ActionResponse<ProductClassification>
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

                return new ActionResponse<ProductClassification>
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

        public async Task<ActionResponse<ProductClassification>> DeleteAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<ProductClassification>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.ProductClassifications.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<ProductClassification>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (model.Delete)
            {
                return new ActionResponse<ProductClassification>
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
                return new ActionResponse<ProductClassification>
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

                return new ActionResponse<ProductClassification>
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

        public async Task<ActionResponse<ProductClassification>> DeleteFullAsync(long id)
        {
            try
            {
                var model = await _context.ProductClassifications.FindAsync(id);
                if (model == null)
                {
                    return new ActionResponse<ProductClassification>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                if (!model.Delete)
                {
                    return new ActionResponse<ProductClassification>
                    {
                        WasSuccess = false,
                        Message = "No se puede eliminar definitivamente hasta que no se elimine previamente"
                    };
                }
                try
                {
                    _context.Remove(model);
                    await _context.SaveChangesAsync();
                    return new ActionResponse<ProductClassification>
                    {
                        WasSuccess = true
                    };
                }
                catch
                {
                    return new ActionResponse<ProductClassification>
                    {
                        WasSuccess = false,
                        Message = "No se pude borrar, porque tiene registros relacionados."
                    };
                }
            }
            catch (Exception ex)
            {

                return new ActionResponse<ProductClassification>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        private ActionResponse<ProductClassification> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<ProductClassification>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<ProductClassification> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<ProductClassification>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}
