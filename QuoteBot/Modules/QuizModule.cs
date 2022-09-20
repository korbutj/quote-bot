using System.Text.RegularExpressions;
using Discord.Commands;
using QuoteBot.Helpers;

namespace QuoteBot.Modules
{
    public class QuizModule : ModuleBase<SocketCommandContext>
    {
        private static Regex TimeRegex = new Regex(@"(\d\d:\d\d)");
        
        [Command("SetQuoteChannel")]
        public Task SetQuoteChannel(string channelName)
        {
            var quoteChannel = this.Context.Guild.Channels.FirstOrDefault(x => x.Name.ToLower() == channelName.ToLower());

            if (quoteChannel is null)
                return ReplyAsync("Kurwa ziomek2000 fix your name (kanał nie istnieje)");
            EnvironmentSettings.SetQuoteChannel(this.Context.Guild.Id, quoteChannel.Id);

            return ReplyAsync($"Gitara siema - kanał z cytatami ustawiony: #{quoteChannel.Name}");
        }

        [Command("SetQuizTime")]
        public Task SetQuizTime(string time)
        {
            if (!TimeRegex.IsMatch(time))
                return ReplyAsync("co to jest - bo na pewno nie poprawny format godziny :thonk:");

            EnvironmentSettings.SetGuildTime(this.Context.Guild.Id, time);


            return ReplyAsync($"git ustawione {time}");
        }

        [Command("GetQuizTime")]
        public Task GetQuizTime()
        {
            return ReplyAsync($"git ustawione {EnvironmentSettings.GetGuildTime(this.Context.Guild.Id)}");
        }

        [Command("StartQuiz")]
        public Task StartQuiz()
        {
            return ReplyAsync("TEST");
        }
    }
}
