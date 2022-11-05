using System.Collections.Concurrent;

namespace QuoteBot.Helpers;

public class ScoreService : IScoreService
{
    //key1 - idGuild
    //key2 - idUser
    private ConcurrentDictionary<ulong, Scoreboard> scores { get; set; } = new ConcurrentDictionary<ulong, Scoreboard>();
    private ConcurrentDictionary<Guid, QuizDetails> quizesResults { get; set; } = new ConcurrentDictionary<Guid, QuizDetails>();

    private const string scoresPath = "./scores.json";
    private const string quizesResultsPath = "./results.json";

    public static ConcurrentDictionary<ulong, Guid> LastQuizIdForGuild { get; set; } = new ConcurrentDictionary<ulong, Guid>(); 

    /// <returns>validation whether user already answered to this question</returns>
    public async Task<bool> AddScore(ulong idGuild, ulong idUser, Guid quizId, bool answeredCorrectly)
    {
        if (quizesResults.ContainsKey(quizId) && quizesResults[quizId].Participants.Any(x => x == idUser))
            return false;
        
        if (!scores.ContainsKey(idUser))
        {
            var scoreboard = new Scoreboard()
            {
                GuessedCorrectly = answeredCorrectly ? 1 : 0, 
                GuessedIncorrectly = answeredCorrectly ? 0 : 1,
                IdUser = idUser
            };
            scores = new ConcurrentDictionary<ulong, Scoreboard>();
            scores.TryAdd(idUser, scoreboard);
        }
        else
        {
            var currentScoreboard = scores[idUser];
            currentScoreboard.GuessedCorrectly += (answeredCorrectly ? 1 : 0);
            currentScoreboard.GuessedIncorrectly += (answeredCorrectly ? 0 : 1);
            scores[idUser] = currentScoreboard;
        }
        
        HandleQuizDetails(idGuild, idUser, quizId, answeredCorrectly);
        
        return true;
    }


    public async Task<Scoreboard> GetScore(ulong userId)
    {
        if (scores.ContainsKey(userId))
            return scores[userId];
        else
            return null;
    }

    public async Task<QuizDetails> GetQuizResults(ulong guildId)
    {
        if (!quizesResults.TryGetValue(LastQuizIdForGuild[guildId], out QuizDetails quizDetails))
        {
            return null;
        }

        return quizDetails;
    }

    public async Task SaveToFile()
    {
        await FileCacher.SaveToFile(scoresPath, scores);
        await FileCacher.SaveToFile(quizesResultsPath, quizesResults);
    }

    public async Task UpdateFromFile()
    {
        scores = await FileCacher.UpdateFromFile<ConcurrentDictionary<ulong, Scoreboard>>(scoresPath);
        quizesResults = await FileCacher.UpdateFromFile<ConcurrentDictionary<Guid, QuizDetails>>(quizesResultsPath);
    }
    
    private void HandleQuizDetails(ulong idGuild, ulong idUser, Guid quizId, bool answeredCorrectly)
    {
        if (quizesResults.ContainsKey(quizId))
        {
            quizesResults[quizId].Participants.Add(idUser);
            if (answeredCorrectly)
            {
                quizesResults[quizId].CorrectAnswers.Add(idUser);
            }
        }
        else
        {
            var correctAnswers = new List<ulong>();
            if(answeredCorrectly)
                correctAnswers.Add(idUser);
            
            quizesResults.TryAdd(quizId, new QuizDetails() { CorrectAnswers = correctAnswers, Participants = new List<ulong>() { idUser } });
            if (LastQuizIdForGuild.ContainsKey(idGuild))
                LastQuizIdForGuild[idGuild] = quizId;
            else
            {
                LastQuizIdForGuild.TryAdd(idGuild, quizId);
            }
        }
    }
}

public class Scoreboard
{
    public ulong IdUser { get; set; }
    public int GuessedCorrectly { get; set; }
    public int GuessedIncorrectly { get; set; }
    public float PercentageGuessed => GuessedCorrectly * 100.0f / (GuessedCorrectly + GuessedIncorrectly) ;
}

public class QuizDetails
{
    public List<ulong> Participants { get; set; }
    public List<ulong> CorrectAnswers { get; set; }
}

public interface IScoreService
{
    Task<bool> AddScore(ulong guildId, ulong idUser, Guid quizId, bool answeredCorrectly);
    Task SaveToFile();
    Task UpdateFromFile();
    Task<Scoreboard> GetScore(ulong userId);
    Task<QuizDetails> GetQuizResults(ulong guildId);
}