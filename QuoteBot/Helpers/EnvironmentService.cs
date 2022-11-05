using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QuoteBot.Models;
using QuoteBot.Services;

namespace QuoteBot.Helpers;
/// <summary>
/// mock for guildService
/// </summary>
public class EnvironmentService : IGuildService
{
    private static readonly IConfiguration config;

    private ConcurrentDictionary<ulong, GuildSettings> guildSettingsMap;
    private ConcurrentDictionary<ulong, List<Citation>> citationsMap;

    private const string GuildSettingsJsonPath = "./guildSettings.json";
    private const string CitationsJsonPath = "./citations.json";

    
    public EnvironmentService()
    {
        guildSettingsMap = new ConcurrentDictionary<ulong, GuildSettings>();
        citationsMap = new ConcurrentDictionary<ulong, List<Citation>>();
    }

    public async Task<ConcurrentDictionary<ulong, GuildSettings>> GetAllGuildSettings()
    {
        return guildSettingsMap;
    }

    public async Task SetQuoteChannel(ulong guildId, ulong idChannel)
    {
        GuildSettings guildSettings;
        if (!guildSettingsMap.ContainsKey(guildId))
        {
            guildSettings = new GuildSettings();
        }
        else
        {
            guildSettings = guildSettingsMap[guildId];
        }
        
        guildSettings.ChannelId = idChannel;
        
        guildSettingsMap[guildId] = guildSettings;
    }

    public async Task SetGuildTime(ulong guildId, string time)
    {
        GuildSettings guildSettings;
        if (!guildSettingsMap.ContainsKey(guildId))
        {
            guildSettings = new GuildSettings();
        }
        else
        {
            guildSettings = guildSettingsMap[guildId];
        }

        guildSettings.QuizTime = time;
        
        guildSettingsMap[guildId] = guildSettings;
    }
    
    public async Task<string> GetGuildTime(ulong guildId)
    {
        GuildSettings guildSettings;
        if (!guildSettingsMap.ContainsKey(guildId))
        {
            guildSettings = new GuildSettings();
            guildSettingsMap[guildId] = guildSettings;

            return guildSettings.QuizTime;
        }
        
        guildSettings = guildSettingsMap[guildId];
        
        return guildSettings.QuizTime;
    }

    public async Task AddCitation(ulong guildId, Citation citation)
    {
        if(this.citationsMap.ContainsKey(guildId))
            this.citationsMap[guildId].Add(citation);
        else
            this.citationsMap.TryAdd(guildId, new List<Citation>() { citation });
    }

    public async Task<Citation> GetRandomCitation(ulong guildId)
    {
        var citationsStored = citationsMap[guildId];
        if (!citationsMap.Any())
            return null;
        
        var rand = new Random();
        return citationsStored.ElementAt(rand.Next() % citationsStored.Count);
    }

    public async Task SaveSettingsToFile()
    {
        await FileCacher.SaveToFile(GuildSettingsJsonPath, guildSettingsMap);
        await FileCacher.SaveToFile(CitationsJsonPath, citationsMap);
    }

    public async Task UpdateGuildSettingsFromFile()
    {
        guildSettingsMap = await FileCacher.UpdateFromFile<ConcurrentDictionary<ulong, GuildSettings>>(GuildSettingsJsonPath);
    }
    
    public async Task UpdateCitationsFromFile()
    {
        citationsMap = await FileCacher.UpdateFromFile<ConcurrentDictionary<ulong, List<Citation>>>(CitationsJsonPath);
    }
}