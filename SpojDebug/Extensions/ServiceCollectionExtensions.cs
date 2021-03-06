﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpojDebug.Core.AppSetting;
using SpojDebug.Data.EF.Repositories.Account;
using SpojDebug.Data.EF.Repositories.AdminSetting;
using SpojDebug.Data.EF.Repositories.Problem;
using SpojDebug.Data.EF.Repositories.Result;
using SpojDebug.Data.EF.Repositories.Submission;
using SpojDebug.Data.EF.Repositories.TestCase;
using SpojDebug.Data.Repositories.Account;
using SpojDebug.Data.Repositories.AdminSetting;
using SpojDebug.Data.Repositories.Problem;
using SpojDebug.Data.Repositories.Result;
using SpojDebug.Data.Repositories.Submission;
using SpojDebug.Data.Repositories.TestCase;
using SpojDebug.Service.Account;
using SpojDebug.Service.Logic.Account;
using SpojDebug.Service.Logic.Problem;
using SpojDebug.Service.Logic.Result;
using SpojDebug.Service.Logic.AdminSetting;
using SpojDebug.Service.Logic.Submission;
using SpojDebug.Service.Logic.TestCase;
using SpojDebug.Service.Problem;
using SpojDebug.Service.Result;
using SpojDebug.Service.SPOJExternal;
using SpojDebug.Service.Submission;
using SpojDebug.Service.TestCase;
using SpojDebug.Business.Result;
using SpojDebug.Business.Logic.Result;
using SpojDebug.Business.Submission;
using SpojDebug.Business.Account;
using SpojDebug.Business.Problem;
using SpojDebug.Business.TestCase;
using SpojDebug.Business.AdminSetting;
using SpojDebug.Business.Logic.AdminSetting;
using SpojDebug.Business.Logic.TestCase;
using SpojDebug.Business.Logic.Problem;
using SpojDebug.Business.Logic.Account;
using SpojDebug.Business.Logic.Submission;
using SpojDebug.Business;
using SpojDebug.Business.Logic;
using SpojDebug.Business.Logic.User;
using SpojDebug.Business.User;
using SpojDebug.Data.EF.Repositories.User;
using SpojDebug.Data.Repositories.User;
using SpojDebug.Service;
using SpojDebug.Service.Logic;
using SpojDebug.Service.Logic.User;
using SpojDebug.Service.User;
using SpojDebug.Business.Cache;
using SpojDebug.Business.Logic.Cache;
using SpojDebug.Data.EF.Contexts;

namespace SpojDebug.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ResolveRepositories(this IServiceCollection services)
        {
            services.AddScoped<IResultRepository, ResultRepository>();
            services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IProblemRepository, ProblemRepository>();
            services.AddScoped<ITestCaseRepository, TestCaseRepository>();
            services.AddScoped<IAdminSettingRepository, AdminSettingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }

        public static void ResolveSingleTonServices(this IServiceCollection services)
        {
            //services.AddSingleton<ISpojExternalService, SpojExternalService>();
        }

        public static void ResolveScopedServices(this IServiceCollection services)
        {
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<ITestCaseService, TestCaseService>();
            services.AddScoped<IAdminSettingService, AdminSettingService>();
            services.AddScoped<ISeedDataService, SeedDataService>();
            services.AddScoped<IUserService, UserService>();
        }

        public static void ResolveScopedBusiness(this IServiceCollection services)
        {
            services.AddScoped<IResultBusiness, ResultBusiness>();
            services.AddScoped<ISubmissionBusiness, SubmissionBusiness>();
            services.AddScoped<IAccountBusiness, AccountBusiness>();
            services.AddScoped<IProblemBusiness, ProblemBusiness>();
            services.AddScoped<ITestCaseBusiness, TestCaseBusiness>();
            services.AddScoped<IAdminSettingBusiness, AdminSettingBusiness>();
            services.AddScoped<ISeedDataBusiness, SeedDataBusiness>();
            services.AddScoped<IUserBusiness, UserBusiness>();

            //Cache
            services.AddScoped<ISubmissionCacheBusiness, SubmissionCacheBusiness>();
            services.AddScoped<IProblemCacheBusiness, ProblemCacheBusiness>();
            services.AddScoped<IAdminSettingCacheBusiness, AdminSettingCacheBusiness>();

            services.AddScoped<SpojDebugDbContext>();
        }
        
    }

    public static class StartingApp
    {
        public static void GetAppSettingConfigs(IConfiguration configuration)
        {
            ApplicationConfigs.SpojKey = configuration.GetSection("SpojKey").Get<SpojKey>();
            ApplicationConfigs.SystemInfo = configuration.GetSection("SystemInfo").Get<SystemInfo>();
            ApplicationConfigs.ConnectionStrings = configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
        }
    } 
}
