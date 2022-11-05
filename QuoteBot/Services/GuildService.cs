using System.Collections.Concurrent;
using QuoteBot.Helpers;
using QuoteBot.Models;

namespace QuoteBot.Services;

public class GuildService : IGuildService
{
    private const string DbName = "quote-bot";
    private const string GuildSettings = "guild-settings";

    public GuildService()
    {
        
    }

    public async Task<ConcurrentDictionary<ulong, GuildSettings>> GetAllGuildSettings()
    {
        return null;
    }

    public async Task SetQuoteChannel(ulong guildId, ulong idChannel)
    {
        
        
    }

    public Task SetGuildTime(ulong guildId, string time)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetGuildTime(ulong guildId)
    {
        throw new NotImplementedException();
    }

    public Task AddCitation(ulong guildId, Citation citation)
    {
        throw new NotImplementedException();
    }

    public Task<Citation> GetRandomCitation(ulong guildId)
    {
        throw new NotImplementedException();
    }

    public Task SaveSettingsToFile()
    {
        throw new NotImplementedException();
    }

    public Task UpdateCitationsFromFile()
    {
        throw new NotImplementedException();
    }

    public Task UpdateGuildSettingsFromFile()
    {
        throw new NotImplementedException();
    }
}