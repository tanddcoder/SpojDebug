using Microsoft.EntityFrameworkCore;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Result;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Entities.TestCase;

namespace SpojDebug.Core.DbContextHelpers
{
    public static class ModelBuilderExtensions
    {
        public static void SetTableNames(this ModelBuilder builder)
        {

            builder.Entity<AccountEntity>().ToTable("Account");
            builder.Entity<ProblemEntity>().ToTable("Problem");
            builder.Entity<ResultEntity>().ToTable("Result");
            builder.Entity<SubmissionEntity>().ToTable("Submission");
            builder.Entity<TestCaseInfoEntity>().ToTable("TestCaseInfo");
            builder.Entity<AdminSettingEntity>().ToTable("AdminSetting");
        }

        public static void SetRuleForEntities(this ModelBuilder builder)
        {

        }
    }
}
