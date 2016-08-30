using System.Data.Entity;
using Edreamer.Framework.Data;
using Edreamer.Framework.Data.EntityFramework;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Domain
{
    public class FrameworkDataContext : DataContext, IFrameworkDataContext
    {
        public IRepository<Module> Modules { get { return Repository<Module>(); } }
        public IRepository<User> Users { get { return Repository<User>(); } }
        public IRepository<Role> Roles { get { return Repository<Role>(); } }
        public IRepository<RolePermission> RolesPermissions { get { return Repository<RolePermission>(); } }
        public IRepository<Media> Media { get { return Repository<Media>(); } }
        public IRepository<Setting> Settings { get { return Repository<Setting>(); } }

        public FrameworkDataContext()
            : base("Framework")
        {
        }

        public FrameworkDataContext(IValidationService validationService)
            : base("Framework")
        {
            Throw.IfArgumentNull(validationService, "validationService");
            //ValidationService = validationService; 
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // ******************** Tables ********************
            modelBuilder.Entity<Module>().ToTable("Framework_Modules");
            modelBuilder.Entity<User>().ToTable("Framework_Users");
            modelBuilder.Entity<Role>().ToTable("Framework_Roles");
            modelBuilder.Entity<RolePermission>().ToTable("Framework_RolesPermissions");
            modelBuilder.Entity<Media>().ToTable("Framework_Media");
            modelBuilder.Entity<Setting>().ToTable("Framework_Settings");

            // ******************** Primary Keys ********************
            modelBuilder.Entity<Module>()
                .HasKey(m => m.Name);
            modelBuilder.Entity<RolePermission>()
                .HasKey(m => new { m.RoleId, m.PermissionName });

            // ******************** Many to Many Relations ********************
            modelBuilder.Entity<User>()
                .HasMany<Role>(u => u.Roles)
                .WithMany(r => r.Users)
                .Map(m =>
                {
                    m.ToTable("Framework_UsersRoles");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}


