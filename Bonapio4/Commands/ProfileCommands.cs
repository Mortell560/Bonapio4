using Bonapio4Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bonapio4.Commands
{
    public class ProfileCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Profiles _profiles;

        public ProfileCommands(Profiles profiles)
        {
            _profiles = profiles;
        }

        [Command("profile"), Alias("p")]
        [Summary("Get user's profile (yours by default)")]
        public async Task Profile(SocketGuildUser user = null)
        {
            Color color = (Color)Utilities.Utilities.RandomColor();

            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = color,
                Description = "*You get Xp by sending messages*",
            };

            if (user == null)
            {
                Profile profile = await _profiles.GetProfileAsync(Context.User.Id, Context.Guild.Id);

                if (!await _profiles.CheckForProfileAsync(Context.User.Id, Context.Guild.Id))
                {
                    await _profiles.CreateProfileAsync(Context.User.Id, Context.Guild.Id);
                    profile = await _profiles.GetProfileAsync(Context.User.Id, Context.Guild.Id);
                }

                builder.WithTitle($"{Context.User.Username}'s Profile");
                builder.WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                builder.AddField("Xp", profile.Xp.ToString(), true);
                builder.AddField("Level", profile.Level.ToString(), true);
            }

            else
            {
                Profile profile = await _profiles.GetProfileAsync(user.Id, Context.Guild.Id);

                if (!await _profiles.CheckForProfileAsync(user.Id, Context.Guild.Id))
                {
                    await _profiles.CreateProfileAsync(user.Id, Context.Guild.Id);
                    profile = await _profiles.GetProfileAsync(user.Id, Context.Guild.Id);
                }

                builder.WithTitle($"{user.Username}'s Profile");
                builder.WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());
                builder.AddField("Xp", profile.Xp.ToString(), true);
                builder.AddField("Level", profile.Level.ToString(), true);
            }

            builder.WithCurrentTimestamp();
            Embed embed = builder.Build();

            await ReplyAsync(null, false, embed);

        }

        [Command("forceProfile"), Alias("fp")]
        [Summary("creates a profile by force (Mods only)")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task ForceProfile(SocketGuildUser user)
        {
            if (await _profiles.CheckForProfileAsync(user.Id, Context.Guild.Id))
            {
                await ReplyAsync("The user already has a profile");
            }
            else
            {
                await _profiles.CreateProfileAsync(user.Id, Context.Guild.Id);
                await ReplyAsync($"{user.Mention} has now a profile even if they didn't want in the first place <:Wut:775344918494445588>");
            }

        }

        [Command("removeProfile"), Alias("rp")]
        [Summary("creates a profile by force (Mods only)")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task RemoveProfile(SocketGuildUser user)
        {
            if (await _profiles.CheckForProfileAsync(user.Id, Context.Guild.Id))
            {
                await _profiles.RemoveProfileAsync(user.Id, Context.Guild.Id);
                await ReplyAsync($"{user.Mention} has now no profile even if they didn't want in the first place <:Wut:775344918494445588>");
            }

            else
            {
                await ReplyAsync($"{Context.User.Mention} are you dumb ? they don't even have a profile <:Confused:750670561272660090>");
            }

        }

        [Command("resetXP"), Alias("rXP")]
        [Summary("Resets XP (same as rp tbh) (Mods only)")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task ResetXp(SocketGuildUser user)
        {
            if (await _profiles.CheckForProfileAsync(user.Id, Context.Guild.Id))
            {
                await _profiles.ResetXpProfileAsync(user.Id, Context.Guild.Id);
                await ReplyAsync($"{user.Mention} has now no Xp even if they didn't want in the first place <:Wut:775344918494445588>");
            }

            else
            {
                await ReplyAsync($"{Context.User.Mention} are you dumb ? they don't even have a profile <:Confused:750670561272660090>");
            }

        }

        [Command("setXP"), Alias("sXP")]
        [Summary("Sets XP for an user's profile, 0 by default (Mods only)")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task SetXp(SocketGuildUser user, int Xp = 0)
        {
            if (await _profiles.CheckForProfileAsync(user.Id, Context.Guild.Id))
            {
                Profile profile = await _profiles.GetProfileAsync(user.Id, Context.Guild.Id);
                int pXP = profile.Xp;

                await _profiles.SetXpProfileAsync(user.Id, Context.Guild.Id, Xp);
                await ReplyAsync($"{user.Mention} had {pXP} Xp and now they have {Xp} Xp even if they didn't want in the first place <:Wut:775344918494445588>");
            }

            else
            {
                await ReplyAsync($"{Context.User.Mention} are you dumb ? they don't even have a profile <:Confused:750670561272660090>");
            }

        }


        [Command("ShowLeaderboard"), Alias("sl")]
        [Summary("Shows the top 7 of the leaderboard for Xp in this server")]
        public async Task LeaderboardAsync()
        {
            Color color = (Color)Utilities.Utilities.RandomColor();

            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = color,
                Title = "**Top 7 Profiles**",
            };

            List<Profile> profiles = await _profiles.GetListProfileAsync(Context.Guild.Id);

            profiles = profiles.OrderByDescending(x => x.Xp).ToList();

            if (profiles != null)
            {
                foreach (Profile profile in profiles)
                {
                    if (Context.Guild.GetUser(profile.UserId) == null)
                    {
                        continue;
                    }

                    SocketGuildUser user = Context.Guild.GetUser(profile.UserId);
                    builder.AddField($"{user.Username}#{user.Discriminator}", profile.Xp.ToString()+" Xp", false);
                }
            }
            else { builder.WithDescription("No User found"); }

            Embed embed = builder.Build();
            await ReplyAsync(null, false, embed);
            
        }

    }
}
