using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using QuoteBot.Helpers;
using QuoteBot.Services;

namespace QuoteBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private IConfiguration _config;
        
        private readonly DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.All,
            AlwaysDownloadUsers = true,
        };
        
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(_socketConfig);
            _config = BuildConfig();

            var services = ConfigureServices();
            await services.GetRequiredService<CommandHandler>().InstallCommandsAsync(services);

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            var environmentService = services.GetRequiredService<EnvironmentService>();
            await environmentService.UpdateCitationsFromFile();
            await environmentService.UpdateGuildSettingsFromFile();
            
            var hostedService = new TimedHostedService(services.GetRequiredService<ILogger<TimedHostedService>>(),services.GetRequiredService<DiscordSocketClient>(), environmentService);
            var cancellationToken = new CancellationToken();
            
            await hostedService.StartAsync(cancellationToken);
            
            var cacheService = new CacheHostedService(services.GetRequiredService<ILogger<CacheHostedService>>(), environmentService);
            await cacheService.StartAsync(cancellationToken);
            
            await Task.Delay(-1);
        }
        
        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                // .AddSingleton<CommandHandlingService>()
                .AddSingleton<CommandHandler>()
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                // Extra
                .AddSingleton(_config)
                .AddScoped<IGuildService, EnvironmentService>()
                // Add additional services here...
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }
    }
}