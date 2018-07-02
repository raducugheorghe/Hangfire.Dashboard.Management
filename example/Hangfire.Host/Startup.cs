
using Chimera.Extensions.Logging.Log4Net;
using Hangfire.Dashboard;
using Hangfire.Dashboard.Management.Extension;
using Hangfire.Host.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;

namespace Hangfire.Host
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfigurationRoot _configuration;


        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _hostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
       
            RegisterServices(services);
            services.AddSession();

          

            var hangfireConnectionString = _configuration.GetConnectionString("HangfireConnection");

            services.AddHangfire(config =>
                { 
                    config.UseLogProvider(new HangfireLogProvider());
                    config.UseSqlServerStorage(hangfireConnectionString, new SqlServer.SqlServerStorageOptions { PrepareSchemaIfNecessary = true});
                    DashboardMetrics.GetMetrics().ToList().ForEach(m => config.UseDashboardMetric(m));
                });


         

            //  services.AddManagementPages(_configuration,_hostingEnvironment, Path.Combine(_hostingEnvironment.ContentRootPath, "Jobs"));

            services.AddManagementPages(_configuration, _hostingEnvironment, typeof(Hangfire.TestJobs.TestJobs).Assembly);

            // Add framework services.
            services.AddMvc();

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 10000000;
            });

            

        }

        private void RegisterServices(IServiceCollection services)
        {

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
         
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddLog4Net(_configuration.GetSection("log4net").Get<Log4NetSettings>());


            app.UseHangfireServer();

            app.UseHangfireDashboard(
                pathMatch: "/dashboard"
            );

            app.UseManagementPages();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
