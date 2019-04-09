using DotNetCoreEfLabo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace DotNetCoreEfLabo
{
    class Program
    {
        private static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            InitializeApplication().Run();
        }

        private static Application InitializeApplication()
        {
            // 環境変数の読み込み
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env))
            {
#if DEBUG
                env = "Development";
#else
                env = "Production"; 
#endif
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // ServiceCollectionの設定
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();
            using (var serviceScope = services.CreateScope())
            {
                try
                {
                    var context = serviceScope.ServiceProvider.GetService<DotNetCorEefLaboContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred initializing the DB.");
                }
            }
            return services.GetService<Application>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // ログの設定       
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            services.AddDbContext<DotNetCorEefLaboContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            // Applicationの設定
            services.AddTransient<Application>();
        }
    }
}
