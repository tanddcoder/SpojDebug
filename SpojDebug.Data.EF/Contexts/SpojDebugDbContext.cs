using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpojDebug.Core.DbContextHelpers;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Result;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Core.User;

namespace SpojDebug.Data.EF.Contexts
{
    public class SpojDebugDbContext : IdentityDbContext<ApplicationUser>
    {
        public SpojDebugDbContext(DbContextOptions options)
            : base(options)
        {
        }
        
        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<ProblemEntity> Problems { get; set; }
        public DbSet<ResultEntity> Results { get; set; }
        public DbSet<SubmissionEntity> Submissions { get; set; }
        public DbSet<TestCaseInfoEntity> TestCases { get; set; }
        public DbSet<AdminSettingEntity> AdminSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.SetTableNames();

            builder.Entity<ProblemEntity>().HasIndex(x => x.SpojId).IsUnique();
            builder.Entity<ResultEntity>().HasIndex(x => new {x.TestCaseSeq, x.SubmmissionId}).IsUnique();
            builder.Entity<SubmissionEntity>().HasIndex(x => x.SpojId).IsUnique();
            builder.Entity<TestCaseInfoEntity>().HasIndex(x => x.ProblemId).IsUnique();
            builder.Entity<AccountEntity>().HasIndex(x => x.SpojUserId).IsUnique();

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        //public override int SaveChanges()
        //{
        //    //var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
        //    //var now = DateTime.Now;
        //    //foreach (var change in modifiedEntities)
        //    //{
        //    //    var entityName = change.Entity.GetType().Name;
        //    //    var primaryKey = GetPrimaryKeyValue(change);

        //    //    foreach (var prop in change.OriginalValues.Properties)
        //    //    {
        //    //        var originalValue = change.OriginalValues[prop].ToString();
        //    //        var currntValue = change.CurrentValues[prop].ToString();
        //    //            //do more
        //    //    }
        //    //}

        //    return base.SaveChanges();
        //}

        //private object GetPrimaryKeyValue(EntityEntry entry)
        //{
        //    var objectStateEntry =
        //        ((IObjectContextAdapter) this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
        //    return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        //}
    }
}
