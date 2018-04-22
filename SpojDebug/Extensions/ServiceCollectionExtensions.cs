using Microsoft.Extensions.DependencyInjection;
using SpojDebug.Data.EF.Repositories.Account;
using SpojDebug.Data.EF.Repositories.Problem;
using SpojDebug.Data.EF.Repositories.Result;
using SpojDebug.Data.EF.Repositories.Submission;
using SpojDebug.Data.EF.Repositories.TestCase;
using SpojDebug.Data.Repositories.Account;
using SpojDebug.Data.Repositories.Problem;
using SpojDebug.Data.Repositories.Result;
using SpojDebug.Data.Repositories.Submission;
using SpojDebug.Data.Repositories.TestCase;
using SpojDebug.Service.Logic.SPOJExternal;
using SpojDebug.Service.SPOJExternal;

namespace SpojDebug.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ResolveRepositories(this IServiceCollection services)
        {
            services.AddScoped<IResultRepository, ResultRepository>();
            services.AddScoped<IResultDetailRepository, ResultDetailRepository>();
            services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IProblemRepository, ProblemRepository>();
            services.AddScoped<ITestCaseRepository, TestCaseRepository>();
        }

        public static void ResolveSingleTonServices(this IServiceCollection services)
        {
            services.AddSingleton<ISpojExternalService, SpojExternalService>();
        }

        public static void ResolveScopedServices(this IServiceCollection services)
        {

        }

        public static void ResolveScopedBusiness(this IServiceCollection services)
        {

        }
    }
}
