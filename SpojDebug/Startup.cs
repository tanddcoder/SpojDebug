using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpojDebug.Core.User;
using SpojDebug.Service.Email;
using SpojDebug.Service.Logic;
using Hangfire;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Service.SPOJExternal;
using SpojDebug.Extensions;
using AutoMapper;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using SpojDebug.Service;
using SpojDebug.Core.AppSetting;

namespace SpojDebug
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            StartingApp.GetAppSettingConfigs(Configuration);

            services.AddDbContext<SpojDebugDbContext>(options =>
                options.UseSqlServer(ApplicationConfigs.ConnectionStrings.DefaultConnection, x => x.MigrationsAssembly("SpojDebug.Data.EF")), ServiceLifetime.Scoped);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SpojDebugDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper();

            // Add Repositories / "Scope"
            services.ResolveRepositories();

            // Add Businesses / "Scoped"
            services.ResolveScopedBusiness();

            // Add Services / "Scoped"
            services.ResolveScopedServices();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            //services.AddSingleton<BaseDbContext>();

            services.AddMvc(config =>
           {
               var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
               config.Filters.Add(new AuthorizeFilter(policy));
           });
            services.AddHangfire(configuration => configuration.UseSqlServerStorage(Configuration["ConnectionStrings:DefaultConnection"]));
            //services.AddHangfire(option => option.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            services.AddMemoryCache();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IAdminSettingService adminService, ISeedDataService seedDataService)
        {
            //GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            var monitor = JobStorage.Current.GetMonitoringApi();
            seedDataService.InitData();
            AppStartBackGroundJob(adminService, monitor);
        }

        private static void AppStartBackGroundJob(IAdminSettingService adminservice, IMonitoringApi monitor)
        {
            PurgeJobs(monitor);
            RecurringJob.AddOrUpdate("GetSpojInfo", () => adminservice.GetSpojInfo(), "*/1 * * * *");
            RecurringJob.AddOrUpdate("DownloadSpojTestCases", () => adminservice.DownloadSpojTestCases(), "*/1 * * * *");
            RecurringJob.AddOrUpdate("GetSubmissionInfo", () => adminservice.GetSubmissionInfo(), "*/1 * * * *");
        }

        public static void PurgeJobs(IMonitoringApi monitor)
        {
            var toDelete = new List<string>();
            foreach (var queue in monitor.ProcessingJobs(0, (int)monitor.ProcessingCount()))
            {
                toDelete.Add(queue.Key);
            }

            foreach (var jobId in toDelete)
            {
                BackgroundJob.Delete(jobId);
            }
        }
    }
}
