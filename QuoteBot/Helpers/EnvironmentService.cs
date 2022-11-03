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

    private Dictionary<ulong, GuildSettings> guildSettingsMap;
    private Dictionary<ulong, List<Citation>> citationsMap;

    private const string GuildSettingsJsonPath = "~/guildSettings.json";
    private const string CitationsJsonPath = "~/citations.json";

    
    public EnvironmentService()
    {
        guildSettingsMap = new Dictionary<ulong, GuildSettings>();
        citationsMap = new Dictionary<ulong, List<Citation>>();
    }

    public async Task<Dictionary<ulong, GuildSettings>> GetAllGuildSettings()
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
            this.citationsMap.Add(guildId, new List<Citation>() { citation });
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
        var guildSettings = JsonConvert.SerializeObject(guildSettingsMap);
        var citations = JsonConvert.SerializeObject(citationsMap);

        await File.WriteAllTextAsync(GuildSettingsJsonPath, guildSettings);
        await File.WriteAllTextAsync(CitationsJsonPath, citations);
    }

    public async Task UpdateGuildSettingsFromFile()
    {
        var settings = await File.ReadAllTextAsync(GuildSettingsJsonPath);

        guildSettingsMap = JsonConvert.DeserializeObject<Dictionary<ulong, GuildSettings>>(settings) ?? new Dictionary<ulong, GuildSettings>();
    }
    
    public async Task UpdateCitationsFromFile()
    {
        var citations = await File.ReadAllTextAsync(CitationsJsonPath);

        citationsMap = JsonConvert.DeserializeObject<Dictionary<ulong, List<Citation>>>(citations) ?? new Dictionary<ulong, List<Citation>>();
    }
}