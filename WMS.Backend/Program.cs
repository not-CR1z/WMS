using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WMS.Backend.Data;
using WMS.Backend.Repositories.Implementations;
using WMS.Backend.Repositories.Implementations.Security;
using WMS.Backend.Repositories.Interfaces.Security;
using WMS.Backend.Repositories.Interfaces;
using WMS.Backend.UnitsOfWork.Implementations;
using WMS.Backend.UnitsOfWork.Interfaces;
using WMS.Backend.UnitsOfWork.Interfaces.Security;
using WMS.Backend.UnitsOfWork.Implementations.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Location;
using WMS.Backend.Repositories.Implementations.Location;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Backend.UnitsOfWork.Implementations.Location;
using WMS.Share.Models.Magister;
using WMS.Backend.Repositories.Implementations.Magister;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Implementations.Magister;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WMS Backend", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
                      Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
                      Example: 'Bearer 12345abcdef'<br /> <br />",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,
            },
            new List<string>()
          }
        });
});
builder.Services.AddDbContext<DataContext>(x=>x.UseSqlServer("name=DbConection"));
builder.Services.AddTransient<SeedDb>();
builder.Services.AddScoped<IFileStorage, FileStorage>();
builder.Services.AddScoped<IValidateSession, ValidateSession>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));

builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<ICountriesUnitOfWork, CountriesUnitOfWork>();

builder.Services.AddScoped<IStatesRepository, StatesRepository>();
builder.Services.AddScoped<IStatesUnitOfWork, StatesUnitOfWork>();

builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();
builder.Services.AddScoped<ICitiesUnitOfWork, CitiesUnitOfWork>();

builder.Services.AddScoped<IDocumentTypeUserRepository, DocumentTypeUserRepository>();
builder.Services.AddScoped<IDocumentTypeUserUnitOfWork, DocumentTypeUserUnitOfWork>();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();

builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
builder.Services.AddScoped<IUserTypeUnitOfWork, UserTypeUnitOfWork>();

builder.Services.AddScoped<IFormUserTypeRepository, FormUserTypeRepository>();
builder.Services.AddScoped<IFormUserTypeUnitOfWork, FormUserTypeUnitOfWork>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyUnitOfWork, CompanyUnitOfWork>();

builder.Services.AddScoped<IBinTypeRepository, BinTypeRepository>();
builder.Services.AddScoped<IBinTypeUnitOfWork, BinTypeUnitOfWork>();

builder.Services.AddScoped<IBranchesRepository, BranchesRepository>();
builder.Services.AddScoped<IBranchesUnitOfWork, BranchesUnitOfWork>();

builder.Services.AddScoped<IWineriesRepository, WineriesRepository>();
builder.Services.AddScoped<IWineriesUnitOfWork, WineriesUnitOfWork>();

builder.Services.AddScoped<ISubWineriesRepository, SubWineriesRepository>();
builder.Services.AddScoped<ISubWineriesUnitOfWork, SubWineriesUnitOfWork>();

builder.Services.AddScoped<IBinsRepository, BinsRepository>();
builder.Services.AddScoped<IBinsUnitOfWork, BinsUnitOfWork>();

builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<IProductTypeUnitOfWork, ProductTypeUnifOfWork>();

builder.Services.AddScoped<IProductClassificationRepository, ProductClassificationRepository>();
builder.Services.AddScoped<IProductClassificationUnitOfWork, ProductClassificationUnifOfWork>();

builder.Services.AddScoped<IProductClassificationDetailRepository, ProductClassificationDetailRepository>();
builder.Services.AddScoped<IProductClassificationDetailUnitOfWork, ProductClassificationDetailUnifOfWork>();

builder.Services.AddScoped<IUMRepository, UMRepository>();
builder.Services.AddScoped<IUMUnitOfWork, UMUnifOfWork>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductUnitOfWork, ProductUnitOfWork>();



builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
    x.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtkey"]!)),
    ClockSkew = TimeSpan.Zero
});
var app = builder.Build();

SeedData(app);
void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory!.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedAsync().Wait();
    }
}

app.UseCors(x => x
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();