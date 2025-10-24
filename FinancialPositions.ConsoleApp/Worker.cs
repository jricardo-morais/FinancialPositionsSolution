using FinancialPositions.ConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinancialPositions.ConsoleApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider; 

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) 
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope()) 
            {
                var dataFetcherService = scope.ServiceProvider.GetRequiredService<DataFetcherService>(); 
                try
                {
                    await dataFetcherService.FetchAndSaveDataAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while fetching and saving data.");
                }
            }

            _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
