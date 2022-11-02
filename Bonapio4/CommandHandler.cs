using Bonapio4Database;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Bonapio4
{
    public class CommandHandler : InitializedService
        {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;
        private readonly Servers _servers;
        private readonly Profiles _profiles;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration config, Servers servers, Profiles profiles)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
            _servers = servers;
            _profiles = profiles;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
            _client.UserJoined += OnUserJoined;

            await _client.SetGameAsync("ruining your life | $help");

            _service.CommandExecuted += OnCommandExecuted;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }


        private async Task OnUserJoined(SocketGuildUser arg)
        {
            Task newTask = new Task(async () => await HandleUserJoined(arg));
            newTask.Start();
            await newTask.ConfigureAwait(true);
        }

        private async Task HandleUserJoined(SocketGuildUser arg)
        {
            if (arg.IsBot) { return; }

            await _profiles.CreateProfileAsync(arg.Id, arg.Guild.Id);
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            SocketUserMessage message = (arg as SocketUserMessage);

            if (message == null) return;

            int argPos = 0;

            //if (CheckForFilo(arg))
            //{
            //    SocketCommandContext _context_ = new SocketCommandContext(_client, message);
            //    await _context_.Channel.SendMessageAsync("Bananans rOtatE fastEr BAnAnas" +
            //        "Go, gO \n We HaVE ReAcheD MaXxiMum VelOcitI", true);
            //    await _context_.Channel.SendMessageAsync("https://tenor.com/view/spinning-banana-gif-22275706");
            //    return;
            //}

            await OnMessageReceivedXp(new SocketCommandContext(_client, message));

            string prefix = await _servers.GetGuildPrefix((message.Channel as SocketGuildChannel).Guild.Id) ?? "$";

            if (!message.HasStringPrefix(prefix, ref argPos) || message.Author.IsBot) return;

            SocketCommandContext context = new SocketCommandContext(_client, message);

            IResult result = await _service.ExecuteAsync(context, argPos, _provider);

            if (!result.IsSuccess) // if an error or a bonapio occurs 
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
                await Log(new LogMessage(LogSeverity.Error, "Gateway", result.ErrorReason));
            }

        }

        private async Task OnMessageReceivedXp(SocketCommandContext context)
        {
            if (!await _profiles.CheckForProfileAsync(context.User.Id, context.Guild.Id)) { await _profiles.CreateProfileAsync(context.User.Id, context.Guild.Id); }

            Profile profile = await _profiles.GetProfileAsync(context.User.Id, context.Guild.Id);
            await GetXpTask(profile, context);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await Log(new LogMessage(LogSeverity.Error, "Error", result.ErrorReason));
            
        }
        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }

        private async Task GetXpTask(Profile profile, SocketCommandContext context)
        {
            if (await _servers.GetSpamAsync(context.Guild.Id) == 0 || context.Channel.Id == await _servers.GetSpamAsync(context.Guild.Id)) { return; }

            var messages = await context.Channel.GetMessagesAsync(10).FlattenAsync();
            IMessage lastMessage = messages.Where(x => x.Author.Id == context.User.Id).OrderByDescending(x => x.CreatedAt).FirstOrDefault();

            if (Math.Abs(lastMessage.CreatedAt.Minute - context.Message.CreatedAt.Minute) < 2) { return; }

            await _profiles.AddXpProfileAsync(profile.UserId, profile.ServerId, 10);
        }
        
        private static bool CheckForFilo(SocketMessage arg)
        {
            GenerateWordsLikeFyleau();
            if (likeFyleau.Any(word => !arg.Content.Contains(word))) return true;
            return false;
        }

        static string[] beginningFyleau = new string[]
        {
          "fi",
          "fy",
          "phy",
          "phi"
        };

        static string[] endFyleau = new string[]
        {
          "leau",
          "lo",
          "lau",
          "loh"
        };
        private static string[] likeFyleau = new string[beginningFyleau.Length * endFyleau.Length];
        public static void GenerateWordsLikeFyleau()
        {
            for (int i = 0; i < beginningFyleau.Length; i++)
            {
                for (int j = 0; j < endFyleau.Length; j++)
                {
                    likeFyleau[i * j] = beginningFyleau[i] + endFyleau[j];
                }
            }

        }

    }
    
}
