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
        
        public IConfiguration Configuration { get; }
        
        public Startup(IHostingEnvironment env) 
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            
            services.AddSession(options => { 
                options.IdleTimeout = TimeSpan.FromMinutes(30); 
                options.CookieName = ".MyApplication";
            });
                
            services.AddMvc();
        }
        
        private static RequestDelegate ChangeContextToHttps(RequestDelegate next)
        {
            return async context =>
            {
                context.Request.Scheme = "https";
                await next(context);
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
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
