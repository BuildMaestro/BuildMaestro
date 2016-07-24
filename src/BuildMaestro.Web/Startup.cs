using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Cors.Infrastructure;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.SignalR;
using BuildMaestro.Web.Hubs;
using Microsoft.Data.Entity;
using BuildMaestro.Web.Core;
using BuildMaestro.Service;
using BuildMaestro.Data.Models;

namespace BuildMaestro.Web
{
    public class Startup
    {
        public static IHubContext BuildAgentHub { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            var dbService = new DatabaseService();
            dbService.Initialize();

            var buildAgent = Core.BuildAgent.Instance;
            if (buildAgent.Service.State != BuildAgent.BuildAgentServiceState.Started)
            {
                buildAgent.Service.Start();
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddEntityFramework().AddSqlServer().AddDbContext<Data.BuildMaestroContext>();

            services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });

            services.AddCors();

            var policy = new CorsPolicy();

            policy.Headers.Add("*");
            policy.Methods.Add("*");
            policy.Origins.Add("*");
            policy.SupportsCredentials = true;

            services.AddCors(x => x.AddPolicy("corsGlobalPolicy", policy));

            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            BuildAgentHub = app.ApplicationServices.GetRequiredService<IHubContext<BuildAgentHub>>();

            app.UseCors("corsGlobalPolicy");

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseSignalR();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
