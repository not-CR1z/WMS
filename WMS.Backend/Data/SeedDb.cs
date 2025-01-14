using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using WMS.Backend.UnitsOfWork.Implementations.Security;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;

namespace WMS.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IConfiguration _configuration;

        public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork, IConfiguration configuration)
        {
            _context = context;
            _usersUnitOfWork = usersUnitOfWork;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            CheckStaticForm();
            await _context.Database.EnsureCreatedAsync();
            await CheckDocumentTypeUserAsync();
            await CheckUserTypeAsync();            
            await CheckUserAsync("123456789", "Administrador", "Sistema", "admin@yopmail.com", "3010000000", "Calle 1 23 34");
            await CheckBinTypeAsync();
            await CheckFormsAsync();
            //await CheckcountriesAsync();
        }

        private void CheckStaticForm()
        {
            if (StaticForm.ListForm == null || StaticForm.ListForm.Count == 0)
            { 
                StaticForm.ListForm = new List<Form>();
                #region Seguridad
                StaticForm.ListForm.Add(new Form { FormCode = 1,Secuence=10, Name = "Roles" });
                StaticForm.ListForm.Add(new Form { FormCode = 2,Secuence=20, Name = "Permisos" });
                #endregion

                #region Maestros
                StaticForm.ListForm.Add(new Form { FormCode = 3,Secuence=10, Name = "Documento Usarios" });
                StaticForm.ListForm.Add(new Form { FormCode = 4,Secuence=20, Name = "Usuarios" });
                StaticForm.ListForm.Add(new Form { FormCode = 7, Secuence = 30, Name = "Tipo Ubicación" });
                StaticForm.ListForm.Add(new Form { FormCode = 12, Secuence = 40, Name = "Tipo Producto" });
                StaticForm.ListForm.Add(new Form { FormCode = 13, Secuence = 50, Name = "Clasificación Producto" });
                StaticForm.ListForm.Add(new Form { FormCode = 14, Secuence = 60, Name = "Unidad Medida" });
                StaticForm.ListForm.Add(new Form { FormCode = 15, Secuence = 70, Name = "Productos" });
                #endregion

                #region Locacion
                StaticForm.ListForm.Add(new Form { FormCode = 5, Secuence = 10, Name = "Paises" });
                StaticForm.ListForm.Add(new Form { FormCode = 6,Secuence=20, Name = "Compañia" });
                StaticForm.ListForm.Add(new Form { FormCode = 8, Secuence = 30, Name = "Sucursales" });
                StaticForm.ListForm.Add(new Form { FormCode = 9, Secuence = 40, Name = "Bodegas" });
                StaticForm.ListForm.Add(new Form { FormCode = 10, Secuence = 50, Name = "Sub-Bodegas" });
                StaticForm.ListForm.Add(new Form { FormCode = 11, Secuence = 60, Name = "Ubicaciones" });
                #endregion
            }
        }        
        private async Task CheckUserTypeAsync()
        {
            if(!_context.UserTypes.Any()) 
            {
                _context.UserTypes.Add(new UserType { Name = "Administrador" });
                await _usersUnitOfWork.CheckRoleAsync("Administrador");
                await _context.SaveChangesAsync();
            }
        }
        private async Task CheckDocumentTypeUserAsync()
        {
            if (!_context.DocumentTypeUsers.Any())
            {
                _context.DocumentTypeUsers.Add(new DocumentTypeUser { Name = "Cedula" });
                await _context.SaveChangesAsync();
            }
        }
        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address)
        {
            var user = await _usersUnitOfWork.GetUserAsync(email);
            if (user == null)
            {
                var userType = await _context.UserTypes.Where(w => w.Name == "Administrador").FirstOrDefaultAsync();
                var documentTypeUser = await _context.DocumentTypeUsers.Where(w => w.Name == "Cedula").FirstOrDefaultAsync();
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    DocumentTypeUserId = documentTypeUser!.Id,
                    SuperU =true,
                };

                await _usersUnitOfWork.AddUserAsync(user, "123456");
                List<long> UserTypesIds =[];
                UserTypesIds.Add(userType!.Id);
                await _usersUnitOfWork.UserToRoleAsync(user, UserTypesIds);
            }

            return user;
        }

        private async Task CheckBinTypeAsync()
        {
            if (!_context.BinTypes.Any())
            {
                var user =await _context.Users.Where(w => w.Email == "admin@yopmail.com").FirstOrDefaultAsync();
                if (user != null)
                {
                    _context.BinTypes.Add(new BinType
                    {
                        Name = "Picking",
                        Description = "Ubicacion almacenamiento productos de mucha ortacion",
                        OrderPicking = 1,
                        CreateDate=DateTime.Now,
                        CreateUserId=user.Id_Local,
                        UpdateUserId=user.Id_Local,
                        UpdateDate=DateTime.Now,
                        Picking=true,
                    });
                    await _context.SaveChangesAsync();

                    _context.BinTypes.Add(new BinType
                    {
                        Name = "SubAlmacenamiento",
                        Description = "Ubicacion almacenamiento",
                        OrderPicking = 2,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.Id_Local,
                        UpdateUserId = user.Id_Local,
                        UpdateDate = DateTime.Now,
                        Picking = true,
                    });
                    await _context.SaveChangesAsync();

                    _context.BinTypes.Add(new BinType
                    {
                        Name = "Almacenamiento",
                        Description = "Ubicacion almacenamiento masivo",
                        OrderPicking = 3,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.Id_Local,
                        UpdateUserId = user.Id_Local,
                        UpdateDate = DateTime.Now,
                        Picking = true,
                    });
                    await _context.SaveChangesAsync();

                    _context.BinTypes.Add(new BinType
                    {
                        Name = "Recepcion",
                        Description = "Ubicacion Recepcion mercancia",
                        OrderPicking = 4,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.Id_Local,
                        UpdateUserId = user.Id_Local,
                        UpdateDate = DateTime.Now,
                        Picking = true,
                    });
                    await _context.SaveChangesAsync();

                    _context.BinTypes.Add(new BinType
                    {
                        Name = "Transito",
                        Description = "Ubicación recepcion transferencias en transito",
                        OrderPicking = 5,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.Id_Local,
                        UpdateUserId = user.Id_Local,
                        UpdateDate = DateTime.Now,
                        Picking = true,
                    });
                    await _context.SaveChangesAsync();

                    _context.BinTypes.Add(new BinType
                    {
                        Name = "Transferencia",
                        Description = "Ubicación recepcion transferencia Directa",
                        OrderPicking = 6,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.Id_Local,
                        UpdateUserId = user.Id_Local,
                        UpdateDate = DateTime.Now,
                        Picking = true,
                    });
                    await _context.SaveChangesAsync();

                    _context.BinTypes.Add(new BinType
                    {
                        Name = "Virtual",
                        Description = "Ubicación Virtual",
                        OrderPicking = 999,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.Id_Local,
                        UpdateUserId = user.Id_Local,
                        UpdateDate = DateTime.Now,
                        Picking = false,
                    });
                    await _context.SaveChangesAsync();
                }
            }
        }
        private async Task CheckFormsAsync()
        {
            FormParent formParent = new();
            var formParents=_context.FormParents.ToList();
            if (!formParents.Where(w=>w.Name=="Seguridad").Any())
            {
                formParent = new FormParent
                {
                    Name = "Seguridad",
                    Icon = "bi bi-shield-lock"
                };  
                _context.FormParents.Add(formParent);
                await _context.SaveChangesAsync();
            }
            if (!formParents.Where(w => w.Name == "Maestros").Any())
            {
                formParent = new FormParent
                {
                    Name = "Maestros",
                    Icon = "bi bi-card-list"
                };
                _context.FormParents.Add(formParent);
                await _context.SaveChangesAsync();
            }
            if (!formParents.Where(w => w.Name == "Locacion").Any())
            {
                formParent = new FormParent
                {
                    Name = "Locacion",
                    Icon = "bi bi-card-list"
                };
                _context.FormParents.Add(formParent);
                await _context.SaveChangesAsync();
            }
            formParents = _context.FormParents.ToList();
            var formSubParents = await _context.FormSubParents.ToListAsync();
            var forms = await _context.Forms.ToListAsync();
            Form form = new();
            FormSubParent formSubParent = new();
            foreach (var item in formParents)
            {
                
                if (item.Name=="Seguridad")
                {
                    formSubParent = formSubParents.Where(w => w.FormParentId == item.Id && w.Name == "Permisos").FirstOrDefault()!;
                    if (formSubParent==null)
                    {
                        formSubParent = new FormSubParent
                        {
                            FormParentId = item.Id,
                            Name = "Permisos",
                            Icon = "bi bi-person",
                            Secuence = 10,
                        };
                        _context.Add(formSubParent);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Roles").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Roles").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Roles",
                            Href = "/usertype",
                            Icon = "bi bi-person-rolodex",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }                   
                    if (!forms.Where(w=>w.FormSubParentId== formSubParent.Id && w.Name=="Permisos").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Permisos").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Permisos",
                            Href = "/formparent",
                            Icon= "bi bi-key",
                            Secuence= formStatic.Secuence, 
                            FormCode= formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                }
                if (item.Name == "Maestros")
                {
                    formSubParent = formSubParents.Where(w => w.FormParentId == item.Id && w.Name == "Usuarios").FirstOrDefault()!;
                    if (formSubParent == null)
                    {
                        formSubParent = new FormSubParent
                        {
                            FormParentId = item.Id,
                            Name = "Usuarios",
                            Icon = "bi bi-person",
                            Secuence = 10,
                        };
                        _context.Add(formSubParent);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Documento Usarios").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Documento Usarios").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Documento Usarios",
                            Href = "/documenttypeusers",
                            Icon = "bi bi-person-vcard",
                            Secuence = formStatic.Secuence,
                            FormCode= formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Usuarios").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Usuarios").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Usuarios",
                            Href = "/users",
                            Icon = "bi bi-person",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                    
                    formSubParent = formSubParents.Where(w => w.FormParentId == item.Id && w.Name == "Inventario").FirstOrDefault()!;
                    if (formSubParent == null)
                    {
                        formSubParent = new FormSubParent
                        {
                            FormParentId = item.Id,
                            Name = "Inventario",
                            Icon = "bi bi-person",
                            Secuence = 10,
                        };
                        _context.Add(formSubParent);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Tipo Ubicación").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Tipo Ubicación").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Tipo Ubicación",
                            Href = "/bintypes",
                            Icon = "bi bi-person",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }


                    formSubParent = formSubParents.Where(w => w.FormParentId == item.Id && w.Name == "Productos").FirstOrDefault()!;
                    if (formSubParent == null)
                    {
                        formSubParent = new FormSubParent
                        {
                            FormParentId = item.Id,
                            Name = "Productos",
                            Icon = "bi bi-person",
                            Secuence = 10,
                        };
                        _context.Add(formSubParent);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Tipo Producto").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Tipo Producto").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Tipo Producto",
                            Href = "/producttypes",
                            Icon = "bi bi-person",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Clasificación Producto").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Clasificación Producto").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Clasificación Producto",
                            Href = "/productclassifications",
                            Icon = "bi bi-person",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Unidad Medida").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Unidad Medida").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Unidad Medida",
                            Href = "/Ums",
                            Icon = "bi bi-person",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Productos").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Productos").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Productos",
                            Href = "/products",
                            Icon = "bi bi-person",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                }
                if (item.Name == "Locacion")
                {
                    formSubParent = formSubParents.Where(w => w.FormParentId == item.Id && w.Name == "Paises").FirstOrDefault()!;
                    if (formSubParent == null)
                    {
                        formSubParent = new FormSubParent
                        {
                            FormParentId = item.Id,
                            Name = "Paises",
                            Icon = "bi bi-person",
                            Secuence = 10,
                        };
                        _context.Add(formSubParent);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Paises").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Paises").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Paises",
                            Href = "/countries",
                            Icon = "bi bi-globe",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }


                    formSubParent = formSubParents.Where(w => w.FormParentId == item.Id && w.Name == "Compañia").FirstOrDefault()!;
                    if (formSubParent == null)
                    {
                        formSubParent = new FormSubParent
                        {
                            FormParentId = item.Id,
                            Name = "Compañia",
                            Icon = "bi bi-person",
                            Secuence = 20,
                        };
                        _context.Add(formSubParent);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Compañia").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Compañia").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Compañia",
                            Href = "/company",
                            Icon = "bi bi-person-vcard",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }

                    formSubParent = formSubParents.Where(w => w.FormParentId == item.Id && w.Name == "Locacion").FirstOrDefault()!;
                    if (formSubParent == null)
                    {
                        formSubParent = new FormSubParent
                        {
                            FormParentId = item.Id,
                            Name = "Locacion",
                            Icon = "bi bi-person",
                            Secuence = 30,
                        };
                        _context.Add(formSubParent);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Sucursales").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Sucursales").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Sucursales",
                            Href = "/branches",
                            Icon = "bi bi-globe",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Bodegas").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Bodegas").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Bodegas",
                            Href = "/wineries",
                            Icon = "bi bi-globe",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Sub-Bodegas").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Sub-Bodegas").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Sub-Bodegas",
                            Href = "/subwineries",
                            Icon = "bi bi-globe",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }
                    if (!forms.Where(w => w.FormSubParentId == formSubParent.Id && w.Name == "Ubicaciones").Any())
                    {
                        var formStatic = StaticForm.ListForm.Where(w => w.Name == "Ubicaciones").FirstOrDefault()!;
                        form = new Form
                        {
                            FormSubParentId = formSubParent.Id,
                            Name = "Ubicaciones",
                            Href = "/bins",
                            Icon = "bi bi-globe",
                            Secuence = formStatic.Secuence,
                            FormCode = formStatic.FormCode
                        };
                        _context.Add(form);
                        await _context.SaveChangesAsync();
                    }

                }
            }
        }
        private async Task CheckUMAsync()
        {
            if (!_context.UMs.Any())
            {
                _context.UMs.Add(new UM { Code = "CAJ", Description = "Caja", QtyDecimal = 0, FactorUnit = 1 });
                _context.UMs.Add(new UM { Code = "CEN", Description = "Centena", QtyDecimal = 0, FactorUnit = 100 });
                _context.UMs.Add(new UM { Code = "CM", Description = "Centimetro", QtyDecimal = 2, FactorUnit = 1 });
                _context.UMs.Add(new UM { Code = "DEC", Description = "Decena", QtyDecimal = 0, FactorUnit = 10 });
                _context.UMs.Add(new UM { Code = "DOC", Description = "Docena", QtyDecimal = 0, FactorUnit = 12 });
                _context.UMs.Add(new UM { Code = "G", Description = "Gramo", QtyDecimal = 2, FactorUnit = 1 });
                _context.UMs.Add(new UM { Code = "KG", Description = "Kilogramo", QtyDecimal = 2, FactorUnit = 1 });
                _context.UMs.Add(new UM { Code = "L", Description = "Litro", QtyDecimal = 2, FactorUnit = 1 });
                _context.UMs.Add(new UM { Code = "M", Description = "Metro", QtyDecimal = 2, FactorUnit = 1 });
                _context.UMs.Add(new UM { Code = "UND", Description = "Unidad", QtyDecimal = 0, FactorUnit = 1 });
                await _context.SaveChangesAsync();
            }
        }
        //private async Task CheckcountriesAsync()
        //{
        //    if (!_context.Countries.Any())
        //    {
        //        var conectionStrings = _configuration.GetConnectionString("DbConection")!;
        //        string conexion = conectionStrings.ToString();
        //        DataSet ds = new DataSet();
        //        SqlCommand comando = new SqlCommand();
        //        SqlDataAdapter aSQL = new SqlDataAdapter();
        //        comando.Connection = new SqlConnection(conexion);

        //        comando.CommandText = WMS.Backend.Properties.Resources.CountriesStatesCities;
        //        aSQL.SelectCommand = comando;
        //        aSQL.Fill(ds);
        //    }
        //}
    }
}
