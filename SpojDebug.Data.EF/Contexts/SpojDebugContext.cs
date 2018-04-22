using Microsoft.EntityFrameworkCore;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Result;
using SpojDebug.Core.Entities.ResultDetail;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Entities.TestCase;

namespace SpojDebug.Data.EF.Contexts
{
    public class SpojDebugDbContext : DbContext
    {
        public SpojDebugDbContext(DbContextOptions<SpojDebugDbContext> options)
       : base(options)
        { }
        public DbSet<AccountEntity> Account { get; set; }
        public DbSet<ProblemEntity> Problem { get; set; }
        public DbSet<ResultEntity> Result { get; set; }
        public DbSet<ResultDetailEntity> ResultDetail { get; set; }
        public DbSet<SubmissionEntity> Submission { get; set; }
        public DbSet<TestCaseEntity> TestCase { get; set; }
    }
}
