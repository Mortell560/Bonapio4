using Bonapio4Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace Bonapio4.Commands
{
    public class ModerationCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Profiles _profiles;
        private readonly Students _students;
        private readonly Servers _servers;

        public ModerationCommands(Profiles profiles, Students students, Servers servers)
        {
            _profiles = profiles;
            _students = students;
            _servers = servers;
        }


        [Command("purge"), Alias("clear")]
        [Summary("The true meaning of censorship")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task purge(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var message = await Context.Channel.SendMessageAsync($"{messages.Count()} messages has been deleted");
            await Task.Delay(3000);
            await message.DeleteAsync();
        }

        [Command("kick")]
        [Summary("kick the specified user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser user, [Remainder]string reason = null)
        {
            if (await _servers.CheckForServerAsync(Context.Guild.Id))
            {
                ulong fetchedChannelId = await _servers.GetLogsAsync(Context.Guild.Id);

                if (fetchedChannelId != 0 && Context.Guild.GetTextChannel(fetchedChannelId) != null)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                        .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                        .WithTitle($"{user.Username} was kicked for the following reason: " + (reason != null ? reason : "none"))
                        .AddField("Id:", user.Id, true)
                        .WithCurrentTimestamp();
                    Embed embed = builder.Build();

                    IMessageChannel logChannel = (IMessageChannel)Context.Client.GetChannel(fetchedChannelId);
                    await logChannel.SendMessageAsync(null, false, embed);
                }

            }

            await ReplyAsync($"Get nae nae'd {user.Mention} <:Bonapio:754044811799035936>");
            await user.KickAsync(reason);
        }

        [Command("ban")]
        [Summary("ban the specified user")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user, [Remainder] string reason = null)
        {
            if (await _servers.CheckForServerAsync(Context.Guild.Id))
            {
                ulong fetchedChannelId = await _servers.GetLogsAsync(Context.Guild.Id);

                if (fetchedChannelId != 0 && Context.Guild.GetTextChannel(fetchedChannelId) != null)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                        .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                        .WithTitle($"{user.Username} was banned for the following reason: " + (reason != null ? reason : "none"))
                        .AddField("Id:", user.Id, true)
                        .WithCurrentTimestamp();
                    Embed embed = builder.Build();

                    IMessageChannel logChannel = (IMessageChannel)Context.Client.GetChannel(fetchedChannelId);
                    await logChannel.SendMessageAsync(null, false, embed);
                }

            }

            if (await _profiles.CheckForProfileAsync(user.Id, Context.Guild.Id)) { await _profiles.RemoveProfileAsync(user.Id, Context.Guild.Id); }
            if (await _students.CheckForStudentAsync(user.Id, Context.Guild.Id)) { await _students.RemoveStudentAsync(user.Id, Context.Guild.Id); }

            await ReplyAsync($"Get nae nae'd {user.Mention} <:Bonapio:754044811799035936>");
            await user?.BanAsync(7, reason);
        }

        [Command("unban")]
        [Summary("unban the specified user")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Unban(ulong userId)
        {
            if (await _servers.CheckForServerAsync(Context.Guild.Id))
            {
                
                ulong fetchedChannelId = await _servers.GetLogsAsync(Context.Guild.Id);

                if (fetchedChannelId != 0 && Context.Guild.GetTextChannel(fetchedChannelId) != null)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                        .WithTitle($"<@{userId}> was unbanned")
                        .AddField("Id:", userId, true)
                        .WithCurrentTimestamp();
                    Embed embed = builder.Build();

                    IMessageChannel logChannel = (IMessageChannel)Context.Client.GetChannel(fetchedChannelId);
                    await logChannel.SendMessageAsync(null, false, embed);
                }

            }

            await ReplyAsync($"c'mere <@{userId}> <:Bonapio:754044811799035936>");
            await Context.Guild.RemoveBanAsync(userId);
        }
    }
}
