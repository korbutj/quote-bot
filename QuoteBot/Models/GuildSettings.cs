namespace QuoteBot.Models;

public class GuildSettings
{
    public ulong ChannelId { get; set; }
    public string QuizTime { get; set; } = "15:00";
}