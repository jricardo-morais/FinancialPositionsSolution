using System.Net.Http;
using System.Text.Json;
using FinancialPositions.Application.Services;
using FinancialPositions.Domain.Entities;
using FinancialPositions.Infrastructure.AppContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FinancialPositions.ConsoleApp.Services
{
    public class DataFetcherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFinancialPositionService _service;
        private readonly ILogger<DataFetcherService> _logger;
        private readonly IConfiguration _configuration;
        private readonly AndbankContext _dbContext;

        public DataFetcherService(
            IHttpClientFactory httpClientFactory,
            IFinancialPositionService service,
            ILogger<DataFetcherService> logger,
            IConfiguration configuration,
            AndbankContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task FetchAndSaveDataAsync()
        {
            try
            {
                _logger.LogInformation("Fetching data from external API...");

                var client = _httpClientFactory.CreateClient("AndbankApiClient");

                using var response = await client.GetAsync("candidate/positions", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                if (response.Content.Headers.ContentLength.HasValue)
                {
                    _logger.LogInformation($"Response Content-Length: {response.Content.Headers.ContentLength.Value} bytes");
                }
                else
                {
                    _logger.LogInformation("Response Content-Length not provided by server; streaming response.");
                }

                await using var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                const int batchSize = 5000;
                var positionsBatchList = new List<FinancialPosition>(batchSize);

                try
                {
                    await foreach (var pos in JsonSerializer.DeserializeAsyncEnumerable<FinancialPosition>(stream, options))
                    {
                        if (pos == null) continue;
                        positionsBatchList.Add(pos);

                        if (positionsBatchList.Count >= batchSize)
                        {
                            await SaveBatchIdempotentAsync(positionsBatchList);
                            positionsBatchList.Clear();
                        }
                    }

                    if (positionsBatchList.Count > 0)
                    {
                        await SaveBatchIdempotentAsync(positionsBatchList);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing API response while streaming.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in FetchAndSaveDataAsync.");
            }
        }

        private async Task SaveBatchIdempotentAsync(List<FinancialPosition> positionsBatch)
        {
            // Assegura que apenas novas posicoes sejam inseridas
            var positionsIds = positionsBatch.Select(x => x.PositionId).ToList();

            // busca as posições que já existem no banco de dados
            var existingIds = await _dbContext.Positions
                .Where(p => positionsIds.Contains(p.PositionId))
                .Select(p => p.PositionId)
                .ToListAsync();

            var positionsToInsert = positionsBatch.Where(x => !existingIds.Contains(x.PositionId)).ToList();

            if (positionsToInsert.Count == 0)
            {
                _logger.LogInformation("All items in batch already exist; skipping insert.");
                return;
            }

            await _service.AddRangeAsync(positionsToInsert);
            _logger.LogInformation($"Inserted {positionsToInsert.Count} new positions (skipped {positionsBatch.Count - positionsToInsert.Count} duplicates).");
        }
    }
}
