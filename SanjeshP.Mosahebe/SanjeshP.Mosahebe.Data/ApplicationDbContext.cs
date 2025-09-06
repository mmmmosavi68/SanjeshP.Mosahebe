using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SanjeshP.Mosahebe.Common.Utilities;
using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.User;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {

        }
        //public DbSet<User> Users { get; set; }
        // سایر DbSetها را در اینجا تعریف کنید
        // public DbSet<Role> Roles { get; set; }
        // public DbSet<Menu> Menus { get; set; }
        // و غیره...
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MyApiDb;Integrated Security=true");
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entitiesAssembly = typeof(IEntity).Assembly;

            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            modelBuilder.AddRestrictDeleteBehaviorConvention();
            modelBuilder.AddSequentialGuidForIdConvention();
            modelBuilder.AddGetDateForCreateDateConvention();
            modelBuilder.AddPluralizingTableNameConvention();

            modelBuilder.Entity<User>()
                .HasOne(u => u.CreatorUser)
                .WithMany()
                .HasForeignKey(u => u.CreatorUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<User>()
    .HasOne(u => u.EditorUser)
    .WithMany()
    .HasForeignKey(u => u.EditorUserId)
    .OnDelete(DeleteBehavior.Restrict);
        }


        public override int SaveChanges()
        {
            _cleanString();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _cleanString();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(cancellationToken);
        }

        // ی و ک عربی و فارسی رو اصلاح شده در بانک ذخیره میکند
        private void _cleanString()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }
    }
}
