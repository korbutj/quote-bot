using System.Text;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using QuoteBot.Helpers;
using QuoteBot.Models;
using QuoteBot.Services;

namespace QuoteBot.Modules
{
    public class QuizModule : ModuleBase<SocketCommandContext>
    {
        private readonly IGuildService _guildService;
        private readonly IScoreService _scoreService;
        private const int fakeAnswers = 3;
        private static Regex TimeRegex = new Regex(@"(\d\d:\d\d)");
        private const string AuthorReplacer = "<autor>";

        public QuizModule(IGuildService guildService, IScoreService scoreService)
        {
            _guildService = guildService;
            _scoreService = scoreService;
        }

        [Command("SetQuoteChannel")]
        public Task SetQuoteChannel(string channelName)
        {
            var quoteChannel = this.Context.Guild.Channels.FirstOrDefault(x => x.Name.ToLower() == channelName.ToLower());

            if (quoteChannel is null)
                return ReplyAsync("Kurwa ziomek2000 fix your name (kanał nie istnieje)");
            _guildService.SetQuoteChannel(this.Context.Guild.Id, quoteChannel.Id);

            return ReplyAsync($"Gitara siema - kanał z cytatami ustawiony: #{quoteChannel.Name}");
        }

        [Command("SetQuizTime")]
        public Task SetQuizTime(string time)
        {
            if (!TimeRegex.IsMatch(time))
                return ReplyAsync("co to jest - bo na pewno nie poprawny format godziny :thonk:");

            _guildService.SetGuildTime(this.Context.Guild.Id, time);


            return ReplyAsync($"git ustawione {time}");
        }

        [Command("GetQuizTime")]
        public Task GetQuizTime()
        {
            return ReplyAsync($"git ustawione {_guildService.GetGuildTime(this.Context.Guild.Id)}");
        }

        [Command("QueryQuizData")]
        public async Task QueryQuizData()
        {
            var regex = new Regex(".{3,32}#[0-9]{4}");
            var messages = (await this.Context.Channel.GetMessagesAsync(1000).ToListAsync()).SelectMany(x => x);

            var users = this.Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "Wewnętrzny krąg".ToLower())?.Members;
            
            foreach (var message in messages.Where(message => message.MentionedUserIds.Any() && !message.Author.IsBot))
            {
                var cleanContent = regex.Replace(message.CleanContent, AuthorReplacer);
                var authors = message.MentionedUserIds.Select(x => new User(){ Id = x, Name = users.FirstOrDefault(z => z.Id == x).DisplayName}).ToList();
                
                await _guildService.AddCitation(this.Context.Guild.Id, new Citation() { Authors = authors, Content = cleanContent, MessageId = message.Id});
            }
            
            await ReplyAsync("baza wirusow programu avast zostala zaaktualizowana");
        }

        [Command("Test")]
        public async Task Test()
        {
            var citation = await _guildService.GetRandomCitation(this.Context.Guild.Id);
            var sessionId = Guid.NewGuid();
            var eventNameBase = $"{citation.MessageId.ToString()}{Globals.Splitter.ToString()}{sessionId}{Globals.Splitter.ToString()}";
            
            var builder = new ComponentBuilder();
            builder.WithButton(string.Join(", ", citation.Authors.Select(x => x.Name)), $"{eventNameBase}true", ButtonStyle.Primary);
            
            var possibleFakeAnswers = this.Context.Guild.Roles
                .FirstOrDefault(x => x.Name.ToLower() == "Wewnętrzny krąg".ToLower())
                ?.Members
                .Where(x => citation.Authors.Select(z => z.Id).All(z => z != x.Id))
                .ToList();
            
            var authorsCount = citation.Authors.Count;
            
            var rand = new Random();
            
            for (int i = 0; i < fakeAnswers; i++)
            {
                var fakeAnswerUsers = possibleFakeAnswers
                    .OrderBy(x => rand.Next())
                    .Take(authorsCount)
                    .ToList();
                builder.WithButton(string.Join(", ", fakeAnswerUsers.Select(x => x.DisplayName)), $"{eventNameBase}false{i}", ButtonStyle.Primary);
            }
            
            await ReplyAsync($"{citation.Content} \n {string.Join(" ", citation.Authors.Select(x => x.Name))}", components: builder.Build());
        }

        [Command("results")]
        public async Task Results()
        {
            var quizDetails = await _scoreService.GetQuizResults(this.Context.Guild.Id);

            var msg = new StringBuilder();

            msg.AppendLine("Wyniki ostatatniego kłizu:");
            msg.AppendLine("✨🎉🎂🎆🎇🎊✨🎉🎂🎆🎇🎊");
            
            var winners = this.Context.Guild.Users.Where(x => quizDetails.CorrectAnswers.Contains(x.Id));
            foreach (var winner in winners)
            {
                msg.AppendLine(winner.Mention);
            }
            msg.AppendLine("✨🎉🎂🎆🎇🎊✨🎉🎂🎆🎇🎊");

            msg.AppendLine("Winners winners chickens dinners!");
            await ReplyAsync(msg.ToString());
        }
    }
}
