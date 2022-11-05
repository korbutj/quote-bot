using System.Collections.Concurrent;

namespace QuoteBot.Helpers;

public class ScoreService : IScoreService
{
    //key1 - idGuild
    //key2 - idUser
    private ConcurrentDictionary<ulong, Scoreboard> scores { get; set; } = new ConcurrentDictionary<ulong, Scoreboard>();

    private const string scoresPath = "./scores.json";
    
    /// <returns>validation whether user already answered to this question</returns>
    public async Task<bool> AddScore(ulong idUser, Guid sessionId, bool answeredCorrectly)
    {
        if (!scores.ContainsKey(idUser))
        {
            var scoreboard = new Scoreboard()
            {
                GuessedCorrectly = answeredCorrectly ? 1 : 0, 
                GuessedIncorrectly = answeredCorrectly ? 0 : 1,
                Answered = new List<Guid>() { sessionId },
                IdUser = idUser
            };
            scores = new ConcurrentDictionary<ulong, Scoreboard>();
            scores.TryAdd(idUser, scoreboard);
        }
        else
        {
            var currentScoreboard = scores[idUser];

            if (currentScoreboard.Answered.Contains(sessionId))
                return false;
            
            currentScoreboard.GuessedCorrectly += (answeredCorrectly ? 1 : 0);
            currentScoreboard.GuessedIncorrectly += (answeredCorrectly ? 0 : 1);
            currentScoreboard.Answered.Add(sessionId);

            scores[idUser] = currentScoreboard;
        }

        return true;
    }
    
    public async Task<Scoreboard> GetScore(ulong userId)
    {
        if (scores.ContainsKey(userId))
            return scores[userId];
        else
            return null;
    }

    public async Task SaveToFile()
    {
        await FileCacher.SaveToFile(scoresPath, scores);
    }

    public async Task UpdateFromFile()
    {
        scores = await FileCacher.UpdateFromFile<ConcurrentDictionary<ulong, Scoreboard>>(scoresPath);
    }
}

public class Scoreboard
{
    public ulong IdUser { get; set; }
    public int GuessedCorrectly { get; set; }
    public int GuessedIncorrectly { get; set; }
    public List<Guid> Answered { get; set; }
    
    public float PercantageGuessed => GuessedCorrectly * 100.0f / (GuessedCorrectly + GuessedIncorrectly) ;
}

public interface IScoreService
{
    Task<bool> AddScore(ulong idUser, Guid sessionId, bool answeredCorrectly);
    Task SaveToFile();
    Task UpdateFromFile();
    Task<Scoreboard> GetScore(ulong userId);
}