namespace QuoteBot.Models;

public class Citation
{
    public ulong MessageId { get; set; }
    public string Content { get; set; }
    public List<User> Authors { get; set; }
}