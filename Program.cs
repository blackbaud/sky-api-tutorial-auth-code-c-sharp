using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Blackbaud.AuthCodeFlowTutorial
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /**
             * To enable local SSL, uncomment the code block below.
             * A self-signed certificate (testCert.pfx) has been provided in the project root.
             */
            var host = new WebHostBuilder()
                .UseKestrel()
                // .UseKestrel(options =>
                // {
                //     options.ThreadCount = 4;
                //     options.NoDelay = true;
                //     options.UseHttps("testCert.pfx", "testPassword");
                //     options.UseConnectionLogging();
                // })
                // .UseUrls("https://localhost:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
