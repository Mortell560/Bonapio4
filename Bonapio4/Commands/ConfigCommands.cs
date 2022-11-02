using Bonapio4Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Bonapio4.Commands
{
    public class ConfigCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Servers _servers;

        public ConfigCommands(Servers servers)
        {
            _servers = servers;
        }

        [Command("prefix", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                string guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? "$";
                await ReplyAsync($"The current prefix for this server is `{guildPrefix}`");
                return;
            }

            if (prefix.Length > 4)
            {
                await ReplyAsync("The lenght of the new prefix is too long and I don't like it");
                return;
            }

            await _servers.ModifyGuildPrefix(Context.Guild.Id, prefix);
            await ReplyAsync($"The prefix changed to `{prefix}`");

            SocketTextChannel channel = (SocketTextChannel)Context.Guild.GetChannel(await _servers.GetLogsAsync(Context.Guild.Id));
            if (channel == null) { return; }
            await channel.SendMessageAsync($"{Context.Guild.Name}'s Prefix has been adjusted by {Context.User.Mention}. the current prefix is now `{prefix}`.");
        }

        [Command("logs")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Logs(string value = null)
        {
            if (value == null)
            {
                if (await _servers.CheckForServerAsync(Context.Guild.Id))
                {
                    ulong fetchedChannelId = await _servers.GetLogsAsync(Context.Guild.Id);
                    if (fetchedChannelId == 0)
                    {
                        await ReplyAsync("There has not been set a logs channel yet!");
                        return;
                    }

                    SocketTextChannel fetchedChannel = Context.Guild.GetTextChannel(fetchedChannelId);
                    if (fetchedChannel == null)
                    {
                        await ReplyAsync("There has not been set a logs channel yet!");
                        await _servers.RemoveLogsAsync(Context.Guild.Id);
                        return;
                    }

                    await ReplyAsync($"The channel used for the logs is set to {fetchedChannel.Mention}");

                    return;
                }
                else
                {
                    await ReplyAsync("The server isn't in the database <:Confused:750670561272660090>");
                    return;
                }
            }

            if (value != "clear")
            {
                if (!MentionUtils.TryParseChannel(value, out ulong parsedId))
                {
                    await ReplyAsync("Please pass in a valid channel! <a:Strokes:810926377737125958>");
                    return;
                }

                SocketTextChannel parsedChannel = Context.Guild.GetTextChannel(parsedId);
                if (parsedChannel == null)
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                else
                {
                    await _servers.ModifyLogsAsync(Context.Guild.Id, parsedId);
                    await ReplyAsync($"Successfully modified the logs channel to {parsedChannel.Mention}");
                    return;
                }


            }

            if (value == "clear")
            {
                if (await _servers.CheckForServerAsync(Context.Guild.Id))
                {
                    await _servers.RemoveLogsAsync(Context.Guild.Id);
                    await ReplyAsync("Successfully cleared the logs channel");
                    return;
                }

                else
                {
                    await ReplyAsync("The server isn't in the database <:Confused:750670561272660090>");
                    return;
                }

            }

            await ReplyAsync("You did not use this command properly. Are you dumb? <a:Strokes:810926377737125958>");
        }

        [Command("spam")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Spam(string value = null)
        {
            if (value == null)
            {
                if (await _servers.CheckForServerAsync(Context.Guild.Id))
                {
                    ulong fetchedChannelId = await _servers.GetSpamAsync(Context.Guild.Id);
                    if (fetchedChannelId == 0)
                    {
                        await ReplyAsync("There has not been set a spam channel yet!");
                        return;
                    }

                    SocketTextChannel fetchedChannel = Context.Guild.GetTextChannel(fetchedChannelId);
                    if (fetchedChannel == null)
                    {
                        await ReplyAsync("There has not been set a spam channel yet!");
                        await _servers.RemoveSpamAsync(Context.Guild.Id);
                        return;
                    }

                    await ReplyAsync($"The channel used for the spam is set to {fetchedChannel.Mention}");

                    return;
                }
                else
                {
                    await ReplyAsync("The server isn't in the database <:Confused:750670561272660090>");
                    return;
                }
            }

            if (value != "clear")
            {
                if (!MentionUtils.TryParseChannel(value, out ulong parsedId))
                {
                    await ReplyAsync("Please pass in a valid channel! <a:Strokes:810926377737125958>");
                    return;
                }

                SocketTextChannel parsedChannel = Context.Guild.GetTextChannel(parsedId);
                if (parsedChannel == null)
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                else
                {
                    await _servers.ModifySpamAsync(Context.Guild.Id, parsedId);
                    await ReplyAsync($"Successfully modified the spam channel to {parsedChannel.Mention}");
                    return;
                }

            }

            if (value == "clear")
            {
                if (await _servers.CheckForServerAsync(Context.Guild.Id))
                {
                    await _servers.RemoveSpamAsync(Context.Guild.Id);
                    await ReplyAsync("Successfully cleared the spam channel");
                    return;
                }

                else
                {
                    await ReplyAsync("The server isn't in the database <:Confused:750670561272660090>");
                    return;
                }

            }

            await ReplyAsync("You did not use this command properly. Are you dumb? <a:Strokes:810926377737125958>");

        }
    }
}
