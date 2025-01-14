using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;

namespace WMS.Backend.Data
{
    public class DataContext:IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options):base(options) 
        { 

        }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<DocumentTypeUser> DocumentTypeUsers { get; set; }
        public DbSet<UserTypeUser> UserTypeUsers { get; set; }
        public DbSet<FormParent> FormParents { get; set; }
        public DbSet<FormSubParent> FormSubParents { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormUserType> FormUserTypes { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Winery> Wineries { get; set; }
        public DbSet<SubWinery> SubWineries { get; set; }
        public DbSet<Bin> Bins { get; set; }
        public DbSet<BinType> BinTypes { get; set; }
        public DbSet<UM> UMs { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductClassification> ProductClassifications { get; set; }
        public DbSet<ProductClassificationDetail> ProductClassificationDetails { get; set; }
        public DbSet<ProductProductClassificationDetail> ProductProductClassificationDetails { get; set; }
        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<State>().HasIndex(x => new { x.CountryId, x.Name }).IsUnique();
            modelBuilder.Entity<City>().HasIndex(x => new { x.StateId, x.Name }).IsUnique();
            modelBuilder.Entity<UserType>().HasIndex(x => new { x.Name }).IsUnique();
            modelBuilder.Entity<DocumentTypeUser>().HasIndex(x => new { x.Name }).IsUnique();
            modelBuilder.Entity<UserTypeUser>().HasIndex(x => new { x.UserTypeId,x.UserIdLocal }).IsUnique();
            modelBuilder.HasSequence<long>("secuence_users");
            modelBuilder.Entity<User>()
                .Property(o => o.Id_Local)
                .HasDefaultValueSql("NEXT VALUE FOR secuence_users");
            DisableCascadingDelete(modelBuilder);
            modelBuilder.Entity<FormParent>().HasIndex(x => new { x.Name }).IsUnique();
            modelBuilder.Entity<FormSubParent>().HasIndex(x => new { x.FormParentId,x.Name }).IsUnique();
            modelBuilder.Entity<Form>().HasIndex(x => new { x.FormSubParentId, x.Name }).IsUnique();
            modelBuilder.Entity<FormUserType>().HasIndex(x => new { x.UserTypeId, x.FormId }).IsUnique();
            modelBuilder.Entity<Branch>().HasIndex(x => new { x.Name }).IsUnique();
            modelBuilder.Entity<Winery>().HasIndex(x => new { x.BranchId,x.Name }).IsUnique();
            modelBuilder.Entity<SubWinery>().HasIndex(x => new { x.WineryId,x.Code }).IsUnique();
            modelBuilder.Entity<Bin>().HasIndex(x => new { x.SubWineryId,x.BinCode }).IsUnique();
            modelBuilder.Entity<BinType>().HasIndex(x => new { x.Name }).IsUnique();
            modelBuilder.Entity<UM>().HasIndex(x => new { x.Code }).IsUnique();
            modelBuilder.Entity<ProductType>().HasIndex(x => new { x.Name }).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(x => new { x.Reference }).IsUnique();
            modelBuilder.Entity<ProductClassification>().HasIndex(x => new { x.Name }).IsUnique();
            modelBuilder.Entity<ProductClassificationDetail>().HasIndex(x => new { x.ProductClassificationId,x.Name }).IsUnique();
            modelBuilder.Entity<ProductProductClassificationDetail>().HasIndex(x => new { x.ProductId, x.ProductClassificationId }).IsUnique();

            modelBuilder.Entity<Product>().Property(u => u.Height).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(u => u.Length).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(u => u.Weight).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(u => u.Width).HasPrecision(18, 2);
        }

        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var relationship in relationships)
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
