using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using QuoteBot.Helpers;

namespace QuoteBot.Services;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private IServiceProvider _serviceProvider;
    
    // Retrieve client and CommandService instance via ctor
    public CommandHandler(DiscordSocketClient client, CommandService commands)
    {
        _commands = commands;
        _client = client;
    }
    
    public async Task InstallCommandsAsync(IServiceProvider services)
    {
        // Hook the MessageReceived event into our command handler
        _client.MessageReceived += HandleCommandAsync;
        _client.ButtonExecuted += HandleButtonClickAsync;
        _serviceProvider = services;
        // Here we discover all of the command modules in the entry 
        // assembly and load them. Starting from Discord.NET 2.0, a
        // service provider is required to be passed into the
        // module registration method to inject the 
        // required dependencies.
        //
        // If you do not use Dependency Injection, pass null.
        // See Dependency Injection guide for more information.
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                                        services: services);
    }

    private async Task HandleButtonClickAsync(SocketMessageComponent component)
    {
        var scoreService = (IScoreService?)_serviceProvider.GetService(typeof(IScoreService));
        var eventNameComponents = component.Data.CustomId.Split(Globals.Splitter);
        
        if (!ulong.TryParse(eventNameComponents[0], out ulong citationId))
            throw new NotImplementedException();
        
        if (!Guid.TryParse(eventNameComponents[1], out Guid sessionId))
            throw new NotImplementedException();

        var anwseredCorrectly = !component.Data.CustomId.Contains("false");

        if (!await scoreService.AddScore(component.GuildId.Value, component.User.Id, sessionId, anwseredCorrectly))
        {
            await component.RespondAsync($"ziomuś już odpowiedziałeś na to pytanie >:C", ephemeral: true);
            return;
        }
        
        var userScoreboard = await scoreService.GetScore(component.User.Id);
        
        if(anwseredCorrectly)
            await component.RespondAsync($"😎 gościu łatwiutko essa\n {userScoreboard.PercentageGuessed:F}% aktualny stan rzeczy", ephemeral: true);
        else
            await component.RespondAsync($"spierdoliłeś :fbsmiley:\n {userScoreboard.PercentageGuessed:F}% aktualny stan rzeczy", ephemeral: true);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix('!', ref argPos) || 
            message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(_client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await _commands.ExecuteAsync(
            context: context, 
            argPos: argPos,
            services: _serviceProvider);
    }
}