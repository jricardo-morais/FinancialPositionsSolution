using FinancialPositions.Domain.Entities;
using FinancialPositions.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FinancialPositions.ConsoleApp.Services
{
    public class DataFetcherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFinancialPositionService _service; 
        private readonly ILogger<DataFetcherService> _logger;
        private readonly IConfiguration _configuration;

        public DataFetcherService(
            IHttpClientFactory httpClientFactory,
            IFinancialPositionService service, 
            ILogger<DataFetcherService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task FetchAndSaveDataAsync()
        {
            try
            {
                _logger.LogInformation("Fetching data from external API...");

                var client = _httpClientFactory.CreateClient("AndbankApiClient");

                //Streamando a resposta ao inves de armazenar todo o conteudo em memoria para evitar atingir HttpClient.Timeout
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

                List<FinancialPosition>? positions = null;
                try
                {
                    positions = await JsonSerializer.DeserializeAsync<List<FinancialPosition>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing API response.");
                }

                if (positions != null && positions.Count > 0)
                {
                    _logger.LogInformation($"Fetched {positions.Count} positions. Saving to database...");

                    // Usando uma abordagem de Batching para grande quantidade de registros
                    const int batchSize = 5000; //Ajusta o tamanho do batch como necessario 
                    for (int i = 0; i < positions.Count; i += batchSize)
                    {
                        var batch = positions.Skip(i).Take(batchSize).ToList();
                        await _service.AddRangeAsync(batch);
                        _logger.LogInformation($"Saved batch {i / batchSize + 1} of {Math.Ceiling((double)positions.Count / batchSize)}.");
                    }
                    _logger.LogInformation("Data saved successfully.");
                }
                else
                {
                    _logger.LogInformation("No positions found or deserialization failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in FetchAndSaveDataAsync.");
            }
        }
    }
}
