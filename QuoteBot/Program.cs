using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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

            var hostedService = new TimedHostedService(services.GetRequiredService<ILogger<TimedHostedService>>(),services.GetRequiredService<DiscordSocketClient>(), new EnvironmentService());
            await hostedService.StartAsync(new CancellationToken());
            
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
                .AddScoped<IMongoClient>(x => new MongoClient(_config["mongoDbConnection"]))
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