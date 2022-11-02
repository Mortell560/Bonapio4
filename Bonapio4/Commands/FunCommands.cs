using Bonapio4Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;

namespace Bonapio4.Commands
{
    public class FunCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Servers _servers;
        private static bool _IsRunning = false;
        private static bool _ShouldRun = true;
        private static int _CallDepth = 0;
        private static int _Queue = 0;
        private static List<ulong> _QueueList = new List<ulong>();
        private static long _GenerationTime = 0; // in ms
        private static int _banana = 0;
        public FunCommands(Servers servers)
        {
            _servers = servers;
        }

        [Command("ping")]
        [Summary("returns pong bc why not?")]
        public async Task PingAsync()
        {
            await ReplyAsync("pong :Bonapio:");
        }

        [Command("8ball"), Alias("Bonapio")]
        [Summary("Ask the magic 8ball a question")]
        public async Task EightBallAsync([Remainder] string question)
        {
            string[] responses = new[]
            {
                "It is certain.",
                "It is decidedly so.",
                "Without a doubt.",
                "Yes - definitely.",
                "You may rely on it.",
                "As I see it, yes.",
                "Most likely.",
                "Outlook good.",
                "Yes.",
                "Signs point to yes.",
                "Reply hazy, try again.",
                "Ask again later.",
                "Better not tell you now.",
                "Cannot predict now.",
                "Concentrate and ask again.",
                "Don't count on it.",
                "My reply is no.",
                "My sources say no.",
                "Outlook not so good.",
                "Very doubtful.",
                "42",
                "43",
                "41"
            };
            Random rand = new Random();
            string result = responses[rand.Next(0, responses.Length)];
            await ReplyAsync(result);
        }

        [Command("pong")]
        [Summary("returns ping bc why not?")]
        public async Task PongAsync()
        {
            await ReplyAsync("ping :Bonapio:");
        }

        [Command("Never"), Alias("gonna", "give", "you", "up")]
        [Summary("Never gonna give you up")]
        public async Task Rick()
        {
            await ReplyAsync("<a:RickRoll:811968091013644289> \n https://cdn.discordapp.com/attachments/368872108979257346/811602785211777034/video0.mp4");
        }

        [Command("playdespacito")]
        [Summary("You seriously don't want to use it")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task Despacito()
        {
            await ReplyAsync("¡Ay! Fonsi, DY Oh, oh no, oh no(oh) Hey yeah Diridiri, dirididi Daddy Go! Sí, sabes que ya llevo un rato mirándote Tengo que bailar contigo hoy(DY) Vi que tu mirada ya estaba llamándome \n Muéstrame el camino que yo voy Oh, tú, tú eres el imán y yo soy el metal Me voy acercando y voy armando el plan Solo con pensarlo se acelera el pulso(oh yeah) Ya, ya me estás gustando más de lo normal Todos mis sentidos van pidiendo más Esto hay que tomarlo sin ningún apuro Despacito Quiero respirar tu cuello despacito Deja que te diga cosas al oído Para que te acuerdes si no estás conmigo Despacito Quiero desnudarte a besos despacito Firmar las paredes de tu laberinto Y hacer de tu cuerpo todo un manuscrito(sube, sube, sube) (Sube, sube) Oh", true);
        }

        [Command("Echo")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Echo(IMessageChannel channel, [Remainder]string message)
        {
            await channel.SendMessageAsync(message);
            var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }

        [Command("meme"), Alias("r", "reddit")]
        [Summary("Well try it <:Wut:775344918494445588>")]
        public async Task meme([Remainder]string subreddit = "dankmemes")
        {
            Color color = (Color)Utilities.Utilities.RandomColor();

            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync($"https://reddit.com/r/{subreddit.Replace(" ", "")}/random.json?limit=1");
            JArray arr = JArray.Parse(result);
            JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            EmbedBuilder builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString())
                .WithColor(color)
                .WithTitle(post["title"].ToString())
                .WithUrl("https://reddit.com" + post["permalink"].ToString())
                .WithFooter($"Upvotes: {post["ups"]}");
            Embed embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("pfp")]
        [Summary("Steals people's pfp (yours by default)")]
        public async Task AvatarAsync(SocketGuildUser user, ushort size = 512)
        {
            await ReplyAsync(CDN.GetUserAvatarUrl(user.Id, user.AvatarId, size, ImageFormat.Auto));
        }

        [Command("pfp")]
        [Summary("Steals people's pfp (yours by default)")]
        public async Task AvatarAsync(ushort size = 512)
        {
            await ReplyAsync(CDN.GetUserAvatarUrl(Context.User.Id, Context.User.AvatarId, size, ImageFormat.Auto));
        }

        [Command("InfoServer"), Alias("Is")]
        [Summary("Sends current Server's info")]
        public async Task InfoServer()
        {
            Color color = (Color)Utilities.Utilities.RandomColor();

            EmbedBuilder builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle($"{Context.Guild.Name}'s infos")
                .WithColor(color)
                .AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Member Count", Context.Guild.MemberCount + " members", true)
                .AddField("Online users", Context.Guild.Users.Where(x => x.Status != UserStatus.Offline).Count() + " members", true)
                .AddField("Boosts", Context.Guild.PremiumSubscriptionCount, true)
                .AddField("Server boost tier", Context.Guild.PremiumTier, true)
                .AddField("Roles",Context.Guild.Roles.Count())
                .AddField("Prefix", await _servers.GetGuildPrefix(Context.Guild.Id) ?? "$", true)
                .AddField("Spam Channel", await _servers.GetSpamAsync(Context.Guild.Id) == 0 ? "No spam channel setup" : Context.Guild.GetTextChannel(await _servers.GetSpamAsync(Context.Guild.Id)).Mention, true)
                .AddField("Logs Channel", await _servers.GetLogsAsync(Context.Guild.Id) == 0 ? "No log channel setup" : Context.Guild.GetTextChannel(await _servers.GetLogsAsync(Context.Guild.Id)).Mention, true)
                .WithCurrentTimestamp();
            Embed embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("InfoUser"), Alias("Iu")]
        [Summary("Sends current user's info")]
        public async Task Info(SocketGuildUser user)
        {
            Color color = (Color)Utilities.Utilities.RandomColor();

            EmbedBuilder builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription($"{user.Username}'s infos")
                .WithColor(color)
                .AddField("User ID", user.Id, true)
                .AddField("Dicriminator", user.Discriminator, true)
                .AddField("Created at", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", user.JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(" ", user.Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp();
            Embed embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        
        [Command("Banana"), Alias("RandomN")]
        [Summary("You don't wanna know")]
        public async Task RandomN(int n = 11)
        {
            Random rnd = new Random();
            int number = rnd.Next(0, n);

            if (number > n/2)
            {
                await ReplyAsync($"Dani said that we're fucked :AllAccordingToCake:");
                return;
            }
            await ReplyAsync($"Dani said that we might be fucked :AllAccordingToCake:");
        }

        [Command("FlameFractal", RunMode = RunMode.Async), Alias("FF")]
        [Summary("Creates and send a fractal")]
        public async Task FlameFractal(int randomFunction, int randomCoefs = 0,int randomColors = 1, int randomSymmetries = 0)
        {
            if (_IsRunning)
            {
                await Context.Channel.SendMessageAsync("Someone is already asking for an image please wait :AllAccordingToCake:");
                return;
            }
            await Context.Channel.SendMessageAsync("Rendering...");

            _IsRunning = true;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string outputFilePath = @"D:\Bonapio4\Bonapio4\Bonapio4\Commands\Release\Banana.png";


            string[] args = {
                outputFilePath,
                randomFunction.ToString(),
                randomCoefs.ToString(),
                randomColors.ToString(),
                randomSymmetries.ToString()
            };

            try
            {
                

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = @"D:\Bonapio4\Bonapio4\Bonapio4\Commands\Release\flame project C++.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = String.Join(' ', args);

                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                System.IO.FileInfo file = new System.IO.FileInfo(outputFilePath);

                //Checks if the image is entirely black or not
                if (file.Length < 25000)
                {
                    _CallDepth++;

                    if (_CallDepth > 10)
                    {
                        await Context.Channel.SendMessageAsync("Sorry i generated too many shitty images bc of your luck, please use the command again");
                        _IsRunning = false;
                        _CallDepth = 0;
                        return;
                    }

                    await Context.Channel.SendMessageAsync($"The image was shitty so im regenerating it ({file.Length} Bytes)");
                    _IsRunning = false;
                    await FlameFractal(randomFunction, randomCoefs, randomColors, randomSymmetries);
                    return;
                }
            }

            catch (Win32Exception ex)
            {
                await ReplyAsync(ex.ToString());
            }

            stopWatch.Stop();
            _CallDepth = 0;
            await Context.Channel.SendFileAsync(outputFilePath, $"Done in {stopWatch.ElapsedMilliseconds} ms using 7 threads");
            _IsRunning = false;
        }

        [Command("FlameFractal", RunMode = RunMode.Async), Alias("FF")]
        [Summary("Creates and send a fractal")]
        public async Task FlameFractal()
        {
            if (_IsRunning)
            {
                await Context.Channel.SendMessageAsync("Someone is already asking for an image please wait :AllAccordingToCake:");
                return;
            }
            await Context.Channel.SendMessageAsync("Rendering...");
            _IsRunning = true;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string outputFilePath = @"D:\Bonapio4\Bonapio4\Bonapio4\Commands\Release\Banana.png";

            try
            {


                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = @"D:\Bonapio4\Bonapio4\Bonapio4\Commands\Release\flame project C++.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                System.IO.FileInfo file = new System.IO.FileInfo(outputFilePath);

                //Checks if the image is entirely black or not
                if (file.Length < 25000)
                {
                    _CallDepth++;

                    if (_CallDepth > 10)
                    {
                        await Context.Channel.SendMessageAsync("Sorry i generated too many shitty images bc of your luck, please use the command again");
                        _IsRunning=false;
                        _CallDepth = 0;
                        return;
                    }

                    await Context.Channel.SendMessageAsync($"The image was shitty so im regenerating it ({file.Length} Bytes)");
                    _IsRunning = false;
                    await FlameFractal();
                    return;
                }
            }

            catch (Win32Exception ex)
            {
                await ReplyAsync(ex.ToString());
            }

            stopWatch.Stop();
            _GenerationTime += stopWatch.ElapsedMilliseconds;
            _CallDepth = 0;
            await Context.Channel.SendFileAsync(outputFilePath, $"Done in {stopWatch.ElapsedMilliseconds} ms using 7 threads");
            _IsRunning = false;
        }

        [Command("FlameFractalN", RunMode = RunMode.Async), Alias("FFN")]
        [Summary("Creates n fractals and send them")]
        public async Task FlameFractalN(int n)
        {

            if (_IsRunning)
            {
                await Context.Channel.SendMessageAsync($"Someone is already asking for {_Queue} image{(_Queue > 1 ? "s" : "")} please wait :AllAccordingToCake:");
                return;
            }

            _Queue = n;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            await Context.Channel.SendMessageAsync($"Rendering your {n} fractals...");
            for (int i = 0; i < n; i++)
            {
                if (!_ShouldRun)
                {
                    _Queue = 0;
                    await Context.Channel.SendMessageAsync("The owner told me to stop so i'll pretend to listen to him :AllAccordingToCake:");
                    break;
                }
                _banana = i + 1;
                await Context.Channel.SendMessageAsync($"{n-i} fractals remaining (Estimated Remaining time: {_Queue * _GenerationTime / _banana} ms) ...");
                await FlameFractal();
                _Queue--;
            }

            _banana = 0;
            _GenerationTime = 0;
            stopWatch.Stop();
            await Context.Channel.SendMessageAsync($"Task done in {stopWatch.ElapsedMilliseconds} ms");
            _ShouldRun = true;
        }


        [Command("FlameFractalC", RunMode = RunMode.Async), Alias("FFC")]
        [Summary("Checks if the queue is empty and tells you when it is empty")]
        public async Task FlameFractalC()
        {
            if (_Queue == 0)
            {
                await ReplyAsync("What are you even waiting for? you can generate fire now");
                return;
            }

            if (_QueueList != null && _QueueList.Contains(Context.User.Id))
            {
                await ReplyAsync($"Are you more stupid than me? Impressive but you're in my list don't worry (you may not be in my database tho) \n {_Queue} fractals in queue (Estimated Remaining time: {_Queue * _GenerationTime / _banana} ms)");
                return;
            }

            _QueueList.Add(Context.User.Id);
            await Context.Channel.SendMessageAsync($"{_Queue} fractals in queue (Estimated Remaining time: {_Queue * _GenerationTime / _banana} ms)");

            while(_Queue != 0)
            {
                Thread.Sleep(200);
            }

            await Context.Channel.SendMessageAsync($"Go ahead {Context.User.Mention}, Queue is empty now :AllAccordingToCake:");
            _QueueList.Remove(Context.User.Id);
        }

        [Command("FFF", RunMode = RunMode.Async)]
        [Summary("Forces fractals to stop generating")]
        [RequireOwner]
        public async Task FlameFractalF()
        {
            _ShouldRun = false;
            await ReplyAsync("Done");
        }

        [Command("Decision")]
        [Summary("The strongest decisions require the strongest random")]
        public async Task Decision()
        {
            Random rnd = new Random();
            int n = rnd.Next() % rnd.Next();
            await ReplyAsync($"you should wait { (n < int.MaxValue/2 ? n.ToString() : "inf")} days, imo");
        }
    }
}
