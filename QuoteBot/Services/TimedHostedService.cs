using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuoteBot.Helpers;

namespace QuoteBot.Services;

public class TimedHostedService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<TimedHostedService> _logger;
    private readonly DiscordSocketClient _client;
    private Timer? _timer = null;
    private const int secondsInterval = 60;
    
    public TimedHostedService(ILogger<TimedHostedService> logger, DiscordSocketClient client)
    {
        _logger = logger;
        _client = client;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(secondsInterval));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var count = Interlocked.Increment(ref executionCount);

        var timeNow = DateTime.Now;
        // var guildsToStart = EnvironmentSettings.configDic.Where(x => TimeSpan.FromHours(double.Parse(x.Value.QuizTime.Split(":").First())) > timeNow.TimeOfDay);

        //
        // _logger.LogInformation(
        //     $"Timed Hosted Service is working. guildId: {guildsToStart.FirstOrDefault().Key.ToString()}");
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