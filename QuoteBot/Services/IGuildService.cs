using System.Collections.Concurrent;
using QuoteBot.Models;

namespace QuoteBot.Services;

public interface IGuildService
{
    Task<ConcurrentDictionary<ulong, GuildSettings>> GetAllGuildSettings();
    Task SetQuoteChannel(ulong guildId, ulong idChannel);

    Task SetGuildTime(ulong guildId, string time);

    Task<string> GetGuildTime(ulong guildId);
    Task AddCitation(ulong guildId, Citation citation);
    Task<Citation> GetRandomCitation(ulong guildId);
    Task SaveSettingsToFile();
    Task UpdateCitationsFromFile();
    Task UpdateGuildSettingsFromFile();
}