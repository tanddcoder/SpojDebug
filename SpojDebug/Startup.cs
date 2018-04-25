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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

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

            services.AddDbContext<SpojDebugDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("SpojDebug.Data.EF")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SpojDebugDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper();

            // Add custom app settings
            services.AddCustomAppSettingConfigs(Configuration);

            // Add Repositories / "Scope"
            services.ResolveRepositories();

            // Add Businesses / "Scoped"
            services.ResolveScopedBusiness();

            // Add Services / "Scoped"
            services.ResolveScopedServices();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            //services.AddSingleton<BaseDbContext>();

            services.AddMvc( config => 
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddHangfire(option => option.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IAdminSettingService adminService)
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

            AppStartBackGroundJob(adminService);
        }

        private void AppStartBackGroundJob(IAdminSettingService adminservice)
        {
            RecurringJob.AddOrUpdate("SPOJBackgroundRecurringJob", () => adminservice.GetSpojInfo(), @"*/5 * * * *");
        }
    }
}
