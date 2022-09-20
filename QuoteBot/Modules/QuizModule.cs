using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using QuoteBot.Helpers;
using QuoteBot.Models;

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
        public async Task StartQuiz()
        {
            var regex = new Regex(".{3,32}#[0-9]{4}");
            var messages = (await this.Context.Channel.GetMessagesAsync(1000).ToListAsync()).SelectMany(x => x);

            var users = this.Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "Wewnętrzny krąg".ToLower())?.Members;
            
            foreach (var message in messages.Where(x => x.MentionedUserIds.Any()))
            {
                var cleanContent = regex.Replace(message.CleanContent, "<zgadnij kotku co mam w srodku>");
                var authors = message.MentionedUserIds.Select(x => new User(){ Id = x, Name = users.FirstOrDefault(z => z.Id == x).DisplayName}).ToList();
                
                EnvironmentSettings.Citations.Add(new Citation() { Authors = authors, Content = cleanContent, MessageId = message.Id});
            }
            
            await ReplyAsync("baza wirusow programu avast zostala zaaktualizowana");
        }
    }
}
