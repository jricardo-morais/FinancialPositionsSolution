using FinancialPositions.ConsoleApp;
using FinancialPositions.ConsoleApp.Services;
using FinancialPositions.Application.Services;
using FinancialPositions.Infrastructure.AppContext;
using FinancialPositions.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

                services.AddDbContext<AndbankContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

                // Register the repository
                services.AddScoped<IFinancialPositionRepository, FinancialPositionRepository>();

                // Register application service
                services.AddScoped<IFinancialPositionService, FinancialPositionService>();

                services.AddHttpClient("AndbankApiClient", client => 
                {
                    var apiUrl = configuration["ApiSettings:Url"];
                    if (string.IsNullOrEmpty(apiUrl))
                    {
                        throw new InvalidOperationException("ApiSettings:Url is not configured.");
                    }
                    client.BaseAddress = new Uri(apiUrl);

                    var apiKey = configuration["ApiSettings:ApiKey"];
                    if (string.IsNullOrEmpty(apiKey))
                    {
                        throw new InvalidOperationException("ApiSettings:ApiKey is not configured.");
                    }
                    client.DefaultRequestHeaders.Add("X-Test-Key", apiKey);
                    client.Timeout = TimeSpan.FromMinutes(5);
                })
                .AddPolicyHandler(GetRetryPolicy());

                services.AddScoped<DataFetcherService>();
                services.AddHostedService<Worker>();
            });

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5));
    }
}

