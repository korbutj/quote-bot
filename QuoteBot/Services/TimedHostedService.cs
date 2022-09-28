using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuoteBot.Helpers;
using QuoteBot.Models;

namespace QuoteBot.Services;

public class TimedHostedService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<TimedHostedService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly IGuildService _guildService;
    private Timer? _timer = null;
    private const int secondsInterval = 60;
    
    public TimedHostedService(ILogger<TimedHostedService> logger, DiscordSocketClient client, IGuildService guildService)
    {
        _logger = logger;
        _client = client;
        _guildService = guildService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(secondsInterval));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        // var count = Interlocked.Increment(ref executionCount);
        var timeNow = DateTime.Now.TimeOfDay;

        var guilds = await _guildService.GetAllGuildSettings();

        foreach (var guild in guilds)
        {
            var timeToExecute = TimeSpan.Parse(guild.Value.QuizTime);
            if (timeToExecute > timeNow && (!guild.Value.LastExecution.HasValue || guild.Value.LastExecution.Value.Date == DateTime.Today))
                ShowQuizPopup(guild);
        }
        
    }

    private void ShowQuizPopup(KeyValuePair<ulong,GuildSettings> guild)
    {
        //todo show quiz popup :)
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}