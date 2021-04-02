using System;
using Blackbaud.AuthCodeFlowTutorial.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Blackbaud.AuthCodeFlowTutorial
{
    public class Startup
    {
        
        /// Stores app settings.
        public IConfiguration Configuration { get; }
        
        
        /// <summary>
        /// Injects app settings from a JSON file.
        /// </summary>
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        
        /// <summary>
        /// Adds services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure app settings so we can inject it into other classes.
            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            
            // Services to be injected.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IConstituentsService, ConstituentsService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddControllersWithViews().AddNewtonsoftJson();

            // Add MVC.
            services.AddMvc();
            
            // Configure session.
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession(options => { 
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.Name = ".AuthCodeFlowTutorial.Session";
            });
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            });
        }


        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseSession();
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
