using System;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Blackbaud.AuthCodeFlowTutorial
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SharedAuthenticationOptions>(options => {
                options.SignInScheme = "ClientCookie";
            });

            services.AddAuthentication();
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
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.Use(ChangeContextToHttps);
            
            var options = new OpenIdConnectOptions {
                RequireHttpsMetadata = true,
                SaveTokens = true,

                // Note: these settings must match the application details
                // inserted in the database at the server level.
                ClientId = "5079f009-4b13-4699-945c-3b584b265694",
                ClientSecret = "pPS8SS0dliufKJISqlgh3Cu7QTUCqdJfVUK95qiIZiU=",
                PostLogoutRedirectUri = "https://localhost:5000",
                CallbackPath = "/auth/callback",
                
                // Use the authorization code flow.
                ResponseType = OpenIdConnectResponseTypes.Code,
                ResponseMode = "query",

                // Note: setting the Authority allows the OIDC client middleware to automatically
                // retrieve the identity provider's configuration and spare you from setting
                // the different endpoints URIs or the token validation parameters explicitly.
                Authority = "https://oauth2.sky.blackbaud.com/"
            };
            
            options.Scope.Clear();
            
            app.UseOpenIdConnectAuthentication(options);
            
            app.UseMvc(RouteConfig.RegisterRoutes);
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
                .UseUrls("https://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
