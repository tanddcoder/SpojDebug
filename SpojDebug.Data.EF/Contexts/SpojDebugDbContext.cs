using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpojDebug.Core.AppSetting;
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

            //builder.Entity<ProblemEntity>().HasKey(x => x.Id);
            //builder.Entity<ProblemEntity>().HasIndex(x => x.SpojId).IsUnique();

            //builder.Entity<ResultEntity>().HasKey(x => x.Id);
            //builder.Entity<ResultEntity>().HasIndex(x => new {x.TestCaseSeq, x.SubmissionId}).IsUnique();
            //builder.Entity<ResultEntity>().HasOne(x => x.Submission).WithMany(x => x.Results).HasForeignKey(x => x.SubmissionId);

            //builder.Entity<SubmissionEntity>().HasKey(x => x.Id);
            //builder.Entity<SubmissionEntity>().HasIndex(x => x.SpojId).IsUnique();
            //builder.Entity<SubmissionEntity>().HasOne(x => x.Account).WithMany(x => x.Submissions).HasForeignKey(x => x.AccountId);
            //builder.Entity<SubmissionEntity>().HasOne(x => x.Problem).WithMany(x => x.Submissions).HasForeignKey(x => x.ProblemId);
            //builder.Entity<SubmissionEntity>().HasMany(x => x.Results);

            //builder.Entity<TestCaseInfoEntity>().HasKey(x => x.Id);
            //builder.Entity<TestCaseInfoEntity>().HasIndex(x => x.ProblemId).IsUnique();
            //builder.Entity<TestCaseInfoEntity>().HasOne(x => x.Problem).WithMany(x => x.TestCaseInfos).HasForeignKey(x => x.ProblemId);

            //builder.Entity<AccountEntity>().HasKey(x => x.Id);
            //builder.Entity<AccountEntity>().HasIndex(x => x.SpojUserId).IsUnique();
            //builder.Entity<AccountEntity>().HasMany(x => x.Submissions);
            //builder.Entity<AccountEntity>().HasOne(x => x.User).WithMany(x => x.Accounts).HasForeignKey(x => x.UserId);

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
