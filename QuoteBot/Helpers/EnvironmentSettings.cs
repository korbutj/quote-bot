﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QuoteBot.Models;

namespace QuoteBot.Helpers;

public static class EnvironmentSettings
{
    private static readonly IConfiguration config;

    private static Dictionary<ulong, GuildSettings> configDic;

    static EnvironmentSettings()
    {
        configDic = new Dictionary<ulong, GuildSettings>();
    }
    
    public static void SetQuoteChannel(ulong guildId, ulong idChannel)
    {
        GuildSettings guildSettings = null;
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

    public static void SetGuildTime(ulong guildId, string time)
    {
        GuildSettings guildSettings = null;
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
    
    public static string GetGuildTime(ulong guildId)
    {
        GuildSettings guildSettings = null;
        if (!configDic.ContainsKey(guildId))
        {
            guildSettings = new GuildSettings();
            configDic[guildId] = guildSettings;

            return guildSettings.QuizTime;
        }
        
        guildSettings = configDic[guildId];
        
        return guildSettings.QuizTime;
    }
}