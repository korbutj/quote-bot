using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuoteBot.Helpers;
using QuoteBot.Models;

namespace QuoteBot.Services;

public class CacheHostedService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<CacheHostedService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly IGuildService _guildService;
    private Timer? _timer = null;
    private const int cacheSaveIntervalSettings = 900;
    
    public CacheHostedService(ILogger<CacheHostedService> logger, IGuildService guildService)
    {
        _logger = logger;
        _guildService = guildService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(cacheSaveIntervalSettings));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        await _guildService.SaveSettingsToFile();
        _logger.LogInformation($"{DateTime.Now:dd/MM/yyyy} - updated settings files");
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cache Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}