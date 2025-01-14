using Microsoft.EntityFrameworkCore;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Security
{
    public class FormUserTypeRepository : GenericRepository<FormUserType>, IFormUserTypeRepository
    {
        private readonly DataContext _context;

        public FormUserTypeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ActionResponse<FormParent>> GetFormParentIdAsync(long Id)
        {
            var queryable = await _context.FormParents.FindAsync(Id);

            if (queryable == null)
            {
                return new ActionResponse<FormParent>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado",
                };
            }

            return new ActionResponse<FormParent>
            {
                WasSuccess = true,
                Result = queryable!
            };
        }

        public async Task<ActionResponse<Form>> GetFormIdAsync(long Id)
        {
            var queryable = await _context.Forms.FindAsync(Id);

            if (queryable == null)
            {
                return new ActionResponse<Form>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado",
                };
            }

            return new ActionResponse<Form>
            {
                WasSuccess = true,
                Result = queryable!
            };
        }
        public async Task<ActionResponse<IEnumerable<FormParent>>> GetFormParentAsync(PaginationDTO pagination)
        {
            var queryable = _context.FormParents.Include(c => c.FormSubParents!).ThenInclude(I=>I.Forms).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<FormParent>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetFormParentTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.FormParents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<Form>>> GetFormAsync(PaginationDTO pagination)
        {
            var queryable = _context.Forms.Where(x => x.FormSubParentId == pagination.Id).Include(I => I.FormUserTypes).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Form>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Secuence)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetFormTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Forms.Where(x => x.FormSubParentId == pagination.Id).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<FormUserType>>> GetFormUserTypeAsync(PaginationDTO pagination)
        {
            var queryable = _context.FormUserTypes.Where(x => x.FormId == pagination.Id).Include(I => I.UserType).Include(I => I.Form).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.UserType!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<FormUserType>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.UserType!.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetFormUserTypeTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.FormUserTypes.Where(x => x.FormId == pagination.Id).Include(I => I.UserType).Include(I => I.Form).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.UserType!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<List<FormParentDTO>>> GetFormParentUser(long Id_local)
        {
            try
            {
                var user = await _context.Users.Where(w => w.Id_Local == Id_local).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new ActionResponse<List<FormParentDTO>>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el usario"
                    };
                }
                if (user.SuperU)
                {
                    var formParentFull = await _context.FormParents.Include(c => c.FormSubParents!).ThenInclude(I => I.Forms).ToListAsync();
                    List<FormParentDTO> formParentsSuperU = new List<FormParentDTO>();
                    foreach (var itemParent in formParentFull)
                    {
                        var parentDTO = new FormParentDTO
                        {
                            Icon = itemParent.Icon,
                            Id = itemParent.Id,
                            Name = itemParent.Name,
                        };
                        List<FormSubParentDTO> formSubParentsSuperU = new List<FormSubParentDTO>();
                        foreach (var itemSubParent in itemParent.FormSubParents!)
                        {
                            var subParentDTO = new FormSubParentDTO
                            {
                                Id = itemSubParent.Id,
                                Name = itemSubParent.Name,
                                Icon = itemSubParent.Icon,
                            };
                            List<FormDTO> formSuperU = new List<FormDTO>();
                            foreach (var item in itemSubParent.Forms!)
                            {
                                var formDTO = new FormDTO
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Href = item.Href,
                                    Icon = item.Icon,
                                    FormSubParentId = item.FormSubParentId,
                                    Secuence = item.Secuence,
                                    FormCode = item.FormCode,
                                    Create = true,
                                    Delete = true,
                                    Read = true,
                                    Update = true,
                                };
                                formSuperU.Add(formDTO);
                            }
                            subParentDTO.Forms= formSuperU;
                            formSubParentsSuperU.Add(subParentDTO);
                        }
                        parentDTO.FormSubParentDTOs= formSubParentsSuperU;
                        formParentsSuperU.Add(parentDTO);
                    }

                    //foreach (var formParent in formParentsSuperU)
                    //{
                    //    formParent.FormSubParentDTOs = (
                    //       from item in formParentFull.Where(w => w.Id == formParent.Id).FirstOrDefault()!.FormSubParents
                    //       select new FormSubParentDTO
                    //       {
                    //           Id = item.Id,
                    //           Name = item.Name,
                    //           Icon = item.Icon,
                    //       }).ToList();

                    //    foreach (var formSubParent in formParent.FormSubParentDTOs)
                    //    {
                    //        formSubParent.Forms = (
                    //       from item in formParentFull.Where(w => w.Id == formParent.Id).FirstOrDefault()!.Forms
                    //       select new FormDTO
                    //       {
                    //           Id = item.Id,
                    //           Name = item.Name,
                    //           Href = item.Href,
                    //           Icon = item.Icon,
                    //           FormParentId = formParent.Id,
                    //           Secuence = item.Secuence,
                    //           FormCode = item.FormCode,
                    //           Create = true,
                    //           Delete = true,
                    //           Read = true,
                    //           Update = true,
                    //       }).ToList();
                    //    }

                       
                    //}
                    return new ActionResponse<List<FormParentDTO>>
                    {
                        WasSuccess = true,
                        Result = formParentsSuperU
                    };
                }

                var userTypeUsers = await _context.UserTypeUsers.Where(w => w.UserIdLocal == Id_local).Include(I => I.UserType).ToListAsync();
                if (userTypeUsers.Count == 0)
                {
                    return new ActionResponse<List<FormParentDTO>>
                    {
                        WasSuccess = true,
                        Result = []
                    };
                }
                var formUserTypes = await _context.FormUserTypes.Include(I => I.Form).ToListAsync();

                var formParents = (
                    from item in userTypeUsers
                    join item2 in formUserTypes on item.UserTypeId equals item2.UserTypeId
                    select new FormParentDTO
                    {
                        Name = item2.Form!.FormSubParent!.FormParent!.Name,
                        Icon = item2.Form.FormSubParent.FormParent.Icon,
                        Id = item2.Form.FormSubParent.FormParent.Id,
                    }).Distinct().ToList();

                var formSubParents = (
                    from item in userTypeUsers
                    join item2 in formUserTypes on item.UserTypeId equals item2.UserTypeId
                    select new FormSubParentDTO
                    {
                        Name = item2.Form!.FormSubParent!.Name,
                        Icon = item2.Form.FormSubParent.Icon,
                        Id = item2.Form.FormSubParent.Id,
                    }).Distinct().ToList();

                var forms = (
                    from item in userTypeUsers
                    join item2 in formUserTypes on item.UserTypeId equals item2.UserTypeId
                    select new FormDTO
                    {
                        Create = item2.Create,
                        Delete = item2.Delete,
                        FormCode = item2.Form!.FormCode,
                        FormSubParentId = item2.Form.FormSubParentId,
                        Href = item2.Form.Href,
                        Icon = item2.Form.Icon,
                        Id = item2.Form.Id,
                        Name = item2.Form.Name,
                        Read = item2.Read,
                        Secuence = item2.Form.Secuence,
                        Update = item2.Update,
                    }).ToList();
                foreach (var item in formSubParents)
                {
                    item.Forms = forms.Where(w => w.FormSubParentId == item.Id).ToList();
                }
                foreach (var item in formParents)
                {
                    item.FormSubParentDTOs = formSubParents.Where(w => w.Forms.Count > 0 && w.Forms[0].FormCode == item.Id).ToList();
                }


                //var formParents = await _context.FormParents.Include(I => I.Forms)!.ThenInclude(I => I.FormUserTypes)!.ToListAsync();

                //var result = (from item in userTypeUsers
                //              join item2 in formUserTypes on item.UserTypeId equals item2.UserTypeId
                //              join item3 in formParents on item2.Form!.FormParentId equals item3.Id
                //              select item3).ToList();

                return new ActionResponse<List<FormParentDTO>>
                {
                    WasSuccess = true,
                    Result = formParents
                };
            }
            catch (Exception ex)
            {

                return new ActionResponse<List<FormParentDTO>>
                {
                    WasSuccess = false,
                    Message = ex.Message!
                };
            }
        }
    }
}
