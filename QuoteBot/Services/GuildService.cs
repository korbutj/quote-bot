using MongoDB.Driver;
using QuoteBot.Helpers;
using QuoteBot.Models;

namespace QuoteBot.Services;

public class GuildService : IGuildService
{
    private readonly IMongoClient _client;

    public GuildService(IMongoClient client)
    {
        _client = client;
    }

    public Task<Dictionary<ulong, GuildSettings>> GetAllGuildSettings()
    {
        throw new NotImplementedException();
    }

    public async Task SetQuoteChannel(ulong guildId, ulong idChannel)
    {
        var mongoDatabase = _client.GetDatabase(Globals.MongoDbName);
        // var guild = mongoDatabase.GetCollection<GuildSettings>(guildId.ToString()).UpdateOne(x => x.Id == guildId, new UpdateOptions<GuildSettings>() {});
        // var res = (await guild.FindAsync(x => x.Id == guildId)).FirstOrDefault(); 
        
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
}