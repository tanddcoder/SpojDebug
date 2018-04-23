using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Result;
using SpojDebug.Core.Entities.ResultDetail;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Core.User;

namespace SpojDebug.Data.EF.Contexts
{
    public class SpojDebugDbContext : IdentityDbContext<ApplicationUser>
    {
        public SpojDebugDbContext(DbContextOptions<SpojDebugDbContext> options)
            : base(options)
        {
        }

        public DbSet<AccountEntity> Account { get; set; }
        public DbSet<ProblemEntity> Problem { get; set; }
        public DbSet<ResultEntity> Result { get; set; }
        public DbSet<ResultDetailEntity> ResultDetail { get; set; }
        public DbSet<SubmissionEntity> Submission { get; set; }
        public DbSet<TestCaseEntity> TestCase { get; set; }
        public DbSet<AdminSettingEntity> AdminSetting { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
