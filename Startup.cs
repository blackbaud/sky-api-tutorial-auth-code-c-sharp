using Blackbaud.AuthCodeFlowTutorial.Services;
using System.Net.Http.Headers;
using System.Text;

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
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // Add Http client for authorizing with SKY API
            services.AddHttpClient<IAuthenticationService, AuthenticationService>("AuthenticationService", client =>
            {
                // Set the base address to the AuthBaseUrl
                client.BaseAddress = new Uri(appSettings.AuthBaseUri);

                // encode the client id and secret
                var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{appSettings.AuthClientId}:{appSettings.AuthClientSecret}"));

                // Add the Authorization header using basic authentication of client id and secret base 64 encoded
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
            });

            // Add Http client for constituent api
            services.AddHttpClient<IConstituentsService, ConstituentsService>("ConstituentService", client =>
            {
                // Set the base address to the SkyApiBaseUri and append constituent/v1/
                client.BaseAddress = new Uri($"{appSettings.SkyApiBaseUri}constituent/v1/");

                // Set request headers for bb-api-subscription-key
                client.DefaultRequestHeaders.Add("bb-api-subscription-key", appSettings.AuthSubscriptionKey);
            });

            // Services to be injected.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IConstituentsService, ConstituentsService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddControllersWithViews();

            // Add MVC.
            services.AddMvc();

            // Configure session.
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
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
