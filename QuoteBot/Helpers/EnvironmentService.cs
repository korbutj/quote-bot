using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QuoteBot.Models;
using QuoteBot.Services;
using GuildSettings = QuoteBot.Models.GuildSettings;

namespace QuoteBot.Helpers;
/// <summary>
/// mock for guildService
/// </summary>
public class EnvironmentService : IGuildService
{
    private static readonly IConfiguration config;

    public Dictionary<ulong, GuildSettings> configDic;
    private Dictionary<ulong, List<Citation>> citations;

    public EnvironmentService()
    {
        configDic = new Dictionary<ulong, GuildSettings>();
        citations = new Dictionary<ulong, List<Citation>>();
    }
    
    public async Task SetQuoteChannel(ulong guildId, ulong idChannel)
    {
        GuildSettings guildSettings;
        if (!configDic.ContainsKey(guildId))
        {
            guildSettings = new GuildSettings();
        }
        else
        {
            guildSettings = configDic[guildId];
        }
        
        guildSettings.ChannelId = idChannel;
        
        configDic[guildId] = guildSettings;
    }

    public async Task SetGuildTime(ulong guildId, string time)
    {
        GuildSettings guildSettings;
        if (!configDic.ContainsKey(guildId))
        {
            guildSettings = new GuildSettings();
        }
        else
        {
            guildSettings = configDic[guildId];
        }

        guildSettings.QuizTime = time;
        
        configDic[guildId] = guildSettings;
    }
    
    public async Task<string> GetGuildTime(ulong guildId)
    {
        GuildSettings guildSettings;
        if (!configDic.ContainsKey(guildId))
        {
            guildSettings = new GuildSettings();
            configDic[guildId] = guildSettings;

            return guildSettings.QuizTime;
        }
        
        guildSettings = configDic[guildId];
        
        return guildSettings.QuizTime;
    }

    public async Task AddCitation(ulong guildId, Citation citation)
    {
        if(this.citations.ContainsKey(guildId))
            this.citations[guildId].Add(citation);
        else
            this.citations.Add(guildId, new List<Citation>() { citation });
    }

    public async Task<Citation> GetRandomCitation(ulong guildId)
    {
        var citationsStored = citations[guildId];
        if (!citations.Any())
            return null;
        
        var rand = new Random();
        return citationsStored.ElementAt(rand.Next() % citationsStored.Count);
    }
}