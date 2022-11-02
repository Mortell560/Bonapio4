using Bonapio4Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Bonapio4.Commands
{
    public class ClubCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Clubs _clubs;

        public ClubCommands(Clubs clubs)
        {
            _clubs = clubs;
        }

        [Command("SeeClubs"), Alias("SC")]
        [Summary("Get all the clubs available from the database")]
        public async Task GetAllClubs()
        {
            var clubs = await _clubs.GetClubsAsync(Context.Guild.Id);
            string description = null;
            Random rnd = new Random();
            Byte[] b = new Byte[3];
            rnd.NextBytes(b);

            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = new Color(b[0], b[1], b[2]),
                Description = "*These are the clubs currently available within this server*",
            };

            foreach (Club club in clubs)
            {
                SocketRole role = Context.Guild.GetRole(club.RoleId);

                if (role == null) { description += $"{club.Name}, "; }

                else { description += $"{role.Mention}, "; }
            }

            builder.AddField("Clubs", description ?? "none", true);
            builder.WithThumbnailUrl(Context.Guild.IconUrl);

            Embed embed = builder.Build();

            await ReplyAsync(null, false, embed);
        }

        [Command("AddClub"), Alias("AC", "AddC")]
        [Summary("Adds a club to the database for this server")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddClub(string Name, string WithRole = null)
        {
            if (await _clubs.CheckForClubAsync(Name.ToLower(), Context.Guild.Id))
            {
                await ReplyAsync("This club already exists. Are you stupid ? <:VoidStare:785184897488781312>");
                return;
            }

            Color color = (Color)Utilities.Utilities.RandomColor();

            if (WithRole.ToLower() == "yes") { await Context.Guild.CreateRoleAsync("Club" + Name, null, color, false, null); }

            await _clubs.CreateClubAsync(Name.ToLower(), Context.Guild.Id);

            await ReplyAsync($"The club {Name} has been created successfully");

        }

        [Command("SetRoleClub"), Alias("SRC", "SetRC")]
        [Summary("Sets a role for a club that exists")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetRoleClub(string Name, ulong roleId)
        {
            if (!await _clubs.CheckForClubAsync(Name.ToLower(), Context.Guild.Id))
            {
                await ReplyAsync("This club doesn't exist. Are you stupid ? <:VoidStare:785184897488781312>");
                return;
            }

            await _clubs.SetRoleClubAsync(Name.ToLower(), Context.Guild.Id, roleId);

            await ReplyAsync($"The club {Name} has been linked to the role {roleId}");
        }

        [Command("RemoveClub"), Alias("RC")]
        [Summary("Remove a club from the server and database")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveClub(string Name, SocketRole role = null)
        {
            if (!await _clubs.CheckForClubAsync(Name.ToLower(), Context.Guild.Id))
            {
                await ReplyAsync("This club doesn't exist. Are you stupid ? <:VoidStare:785184897488781312>");
                return;
            }

            await _clubs.RemoveClubAsync(Name.ToLower(), Context.Guild.Id);

            if (role != null) { await role.DeleteAsync(); }

            await ReplyAsync($"The club {Name} has been deleted successfully");
        }
    }
}
