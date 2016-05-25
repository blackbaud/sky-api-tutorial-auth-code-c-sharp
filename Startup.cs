using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Blackbaud.AuthCodeFlowTutorial.Services;

namespace Blackbaud.AuthCodeFlowTutorial
{
    public class Startup
    {
        
        /// Stores app settings.
        public IConfiguration Configuration { get; }
        
        
        /// <summary>
        /// Injects app settings from a JSON file.
        /// </summary>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");
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
            
            // Configure the session.
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession(options => { 
                options.IdleTimeout = TimeSpan.FromMinutes(30); 
                options.CookieName = ".MyApplication";
            });
            
            services.AddMvc();
            
            // Include our authentication service.
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            
            // Let the HTTP context be injected into non-constructors.
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
        }
        
        
        /// <summary>
        /// Forces SSL.
        /// </summary>
        private static RequestDelegate ChangeContextToHttps(RequestDelegate next)
        {
            return async context =>
            {
                context.Request.Scheme = "https";
                await next(context);
            };
        }


        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseSession();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.Use(ChangeContextToHttps);
            app.UseMvc(routes =>
                 routes.MapRoute(
                    name: "Default",
                    template: "{controller=Home}/{action=Index}"
                )
            );
        }


        /// <summary>
        /// Initializes the server and app.
        /// </summary>
        public static void Main(string[] args)
        {
            IWebHost host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.ThreadCount = 4;
                    options.NoDelay = true;
                    options.UseHttps("testCert.pfx", "testPassword");
                    options.UseConnectionLogging();
                })
                .UseUrls("https://localhost:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
