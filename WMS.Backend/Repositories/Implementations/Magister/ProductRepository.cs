using Microsoft.EntityFrameworkCore;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;
using System.Linq.Dynamic.Core;
using WMS.Share.Helpers;
using WMS.Share.Models.Location;

namespace WMS.Backend.Repositories.Implementations.Magister
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        public ProductRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ActionResponse<Product>> AddAsync(Product model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            try
            {
                var productType = await _context.ProductTypes.FindAsync(model.ProductTypeId);
                if (productType == null)
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra tipo Producto"
                    };
                }
                model.CreateUserId = Id_Local;
                model.CreateDate = DateTime.Now;
                model.UpdateDate = DateTime.Now;
                model.UpdateUserId = Id_Local;
                _context.Add(model);               
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
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

                return new ActionResponse<Product>
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

        public async Task<ActionResponse<List<Product>>> AddListAsync(List<Product> list, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<List<Product>>
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
                    var productType = await _context.ProductTypes.AsNoTracking().Where(w => w.Name == item.GenericSearchName1).FirstOrDefaultAsync();
                    if (productType == null)
                    {
                        item.StrError = "No se encuentra Tipo producto" + item.GenericSearchName1;
                        continue;
                    }
  
                    if (item.Update == false)
                    {
                        item.ProductTypeId = productType.Id;
                        item.CreateUserId = Id_Local;
                        item.CreateDate = DateTime.Now;
                        item.UpdateDate = DateTime.Now;
                        item.UpdateUserId = Id_Local;
                        _context.Add(item);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        Product? model=null;
                        if(item.Id != 0)
                        {
                            model = await _context.Products.Where(w => w.Delete == false && w.Id == item.Id).FirstOrDefaultAsync();
                            if (model == null)
                            {
                                item.StrError = "No se encuentra el registro";
                                continue;
                            }                            
                        }
                        else
                        {
                            model = await _context.Products.Where(w => w.Delete == false && w.Reference == item.Reference).FirstOrDefaultAsync();
                            if (model == null)
                            {
                                item.StrError = "No se encuentra el registro";
                                continue;
                            }
                        }
    
                        model.ProductTypeId = productType.Id;
                        model.Reference = item.Reference;
                        model.Description = item.Description;
                        model.ExternalCode = item.ExternalCode;
                        model.IsKey = item.IsKey;
                        model.Fdimen = item.Fdimen;
                        model.WithLot = item.WithLot;
                        model.WithSerial = item.WithSerial;
                        model.Length = item.Length;
                        model.Width = item.Width;
                        model.Height = item.Height;
                        model.Weight = item.Weight;
                        model.Active = item.Active;
                        model.UpdateUserId = Id_Local;
                        model.UpdateDate = DateTime.Now;
                        _context.Update(model);
                        await _context.SaveChangesAsync();
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
                return new ActionResponse<List<Product>>
                {
                    WasSuccess = false,
                    Message = "Revisar Listado de errores",
                    Result = listerror
                };
            }
            await _context.SaveChangesAsync();
            transaction.Commit();
            return new ActionResponse<List<Product>>
            {
                WasSuccess = true,
                Message = "No se encuentra usuario",
                Result = list,
            };
        }

        public async Task<ActionResponse<Product>> DeleteAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.Products.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (model.Delete)
            {
                return new ActionResponse<Product>
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
                return new ActionResponse<Product>
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

                return new ActionResponse<Product>
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

        public async Task<ActionResponse<Product>> DeleteClasificationAsync(long id)
        {
            try
            {
                var model = await _context.ProductProductClassificationDetails.FindAsync(id);
                if (model == null)
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                try
                {
                    _context.ProductProductClassificationDetails.Remove(model);
                    _context.SaveChanges();
                    return new ActionResponse<Product>
                    {
                        WasSuccess = true
                    };
                }
                catch
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "No se pude borrar, porque tiene registros relacionados."
                    };
                }
            }
            catch (Exception ex)
            {

                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<Product>> ActiveAsync(long id, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.Products.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (!model.Delete)
            {
                return new ActionResponse<Product>
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
                return new ActionResponse<Product>
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

                return new ActionResponse<Product>
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

        public async Task<ActionResponse<Product>> DeleteFullAsync(long id)
        {
            try
            {
                var model = await _context.Products.FindAsync(id);
                if (model == null)
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                if (!model.Delete)
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "No se puede eliminar definitivamente hasta que no se elimine previamente"
                    };
                }
                try
                {
                    _context.Remove(model);
                    await _context.SaveChangesAsync();
                    return new ActionResponse<Product>
                    {
                        WasSuccess = true
                    };
                }
                catch
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "No se pude borrar, porque tiene registros relacionados."
                    };
                }
            }
            catch (Exception ex)
            {

                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<Product>> GetAsync(long id)
        {
            var model = await _context.Products.Include(I => I.ProductType).FirstOrDefaultAsync(w => w.Id == id);
            if (model == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            var productProductClassificationDetails =await _context.ProductProductClassificationDetails.Where(w => w.ProductId == model.Id).Include(I=>I.ProductClassification).ThenInclude(I=>I!.ProductClassificationDetails).OrderBy(o=>o.ProductClassification!.Name).ToListAsync();

            foreach (var item in productProductClassificationDetails)
            {
                item.ListDetail = await _context.ProductClassificationDetails.Where(w => w.ProductClassificationId == item.ProductClassificationId).ToListAsync();
            }
            model.ProductProductClassificationDetails = productProductClassificationDetails;
            return new ActionResponse<Product>
            {
                WasSuccess = true,
                Result = model
            };
        }

        public async Task<ActionResponse<IEnumerable<Product>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = await _context.Products.Where(w => w.Delete == false).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.Where(w => w.Delete == false).Include(I => I.ProductType).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.ProductTypeId.ToString() == pagination.Filter);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.Reference.ToString().Contains(pagination.Filter1.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Description.ToString().Contains(pagination.Filter2.ToLower()));
            }
            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Reference).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Product>>> DownloadAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.Where(w => w.Delete == false).Include(I => I.ProductType).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.ProductTypeId.ToString() == pagination.Filter);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.Reference.ToString().Contains(pagination.Filter1.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Description.ToString().Contains(pagination.Filter2.ToLower()));
            }
            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Reference).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.Where(w => w.Delete == false).Include(I => I.ProductType).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.ProductTypeId.ToString() == pagination.Filter);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.Reference.ToString().Contains(pagination.Filter1.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Description.ToString().Contains(pagination.Filter2.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<Product>>> GetDeleteAsync()
        {
            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = await _context.Products.Where(w => w.Delete == true).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Product>>> GetDeleteAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.Where(w => w.Delete == true).Include(I => I.ProductType).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.ProductTypeId.ToString() == pagination.Filter);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.Reference.ToString().Contains(pagination.Filter1.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Description.ToString().Contains(pagination.Filter2.ToLower()));
            }
            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.Where(w => w.Delete == true).Include(I => I.ProductType).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.ProductTypeId.ToString() == pagination.Filter);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.Reference.ToString().Contains(pagination.Filter1.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter2))
            {
                queryable = queryable.Where(x => x.Description.ToString().Contains(pagination.Filter2.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<Product>> UpdateAsync(Product model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            if (model == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }

            try
            {
                if (model.ProductProductClassificationDetails!.Count > 0)
                {
                    foreach (var item in model.ProductProductClassificationDetails)
                    {
                        var productProductClassificationDetail = await _context.ProductProductClassificationDetails.Where(w => w.ProductId == item.ProductId && w.ProductClassificationId == item.ProductClassificationId).FirstOrDefaultAsync();
                        if (productProductClassificationDetail == null)
                        {
                            var productProductClassificationDetailNew = new ProductProductClassificationDetail
                            {
                                ProductId = model.Id,
                                ProductClassificationId = item.ProductClassificationId,
                                ProductClassificationDetailId = item.ProductClassificationDetailId,
                                CreateDate = DateTime.Now,
                                CreateUserId = Id_Local,
                                UpdateDate = DateTime.Now,
                                UpdateUserId = Id_Local
                            };
                            _context.Add(productProductClassificationDetailNew);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            if (productProductClassificationDetail.ProductClassificationDetailId != item.ProductClassificationDetailId)
                            {
                                productProductClassificationDetail.ProductClassificationDetailId = item.ProductClassificationDetailId;
                                productProductClassificationDetail.UpdateDate = DateTime.Now;
                                productProductClassificationDetail.UpdateUserId = Id_Local;
                                _context.Update(productProductClassificationDetail);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                var product=await _context.Products.FindAsync(model.Id);
                if(product == null)
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                product.ProductTypeId = model.ProductTypeId;
                product.Reference = model.Reference;
                product.Description = model.Description;
                product.ExternalCode = model.ExternalCode;
                product.IsKey = model.IsKey;
                product.Fdimen = model.Fdimen;
                product.WithLot = model.WithLot;
                product.WithSerial = model.WithSerial;
                product.Length = model.Length;
                product.Width = model.Width;
                product.Height = model.Height;
                product.Weight = model.Weight;
                product.Active = model.Active;
                model.UpdateDate = DateTime.Now;
                model.UpdateUserId = Id_Local;
                _context.Update(product);
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
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
                return new ActionResponse<Product>
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

        private ActionResponse<Product> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<Product>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<Product> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<Product>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}
