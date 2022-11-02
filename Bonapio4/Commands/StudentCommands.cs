using Bonapio4Database;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bonapio4.Commands
{
    public class StudentCommands: InteractiveBase
    {
        private readonly Clubs _clubs;
        private readonly Students _students;

        public StudentCommands(Clubs clubs, Students students)
        {
            _clubs = clubs;
            _students = students;
        }

        [Command("SeeStudent"), Alias("ss")]
        [Summary("Get student's profile (yours by default)")]
        public async Task Student(SocketGuildUser user = null)
        {
            Color color = (Color)Utilities.Utilities.RandomColor();

            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = color,
            };

            if (user == null)
            {
                if (!await _students.CheckForStudentAsync(Context.User.Id, Context.Guild.Id))
                {
                    await ReplyAsync("You don't have any student profile! please use the verif command");
                    return;
                }

                Student student = await _students.GetStudentAsync(Context.User.Id, Context.Guild.Id);
                string desc = null;

                if (student.Club != null)
                {
                    foreach (Club club in student.Club)
                    {
                        SocketRole role = Context.Guild.GetRole(club.RoleId);

                        if (role == null) { desc += $"{club.Name}, "; }

                        else { desc += $"{role.Mention}, "; }
                    }
                }

                builder.WithTitle($"{Context.User.Username}'s Student profile");
                builder.WithFooter(Context.User.Id.ToString(), Context.Guild.IconUrl);
                builder.ThumbnailUrl = Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl();
                builder.AddField("Name", student.Name, false);
                builder.AddField("Surname", student.Surname, false);
                builder.AddField("Grade", student.Grade, false);
                builder.AddField("Clubs", desc ?? "none", false);
            }

            else
            {
                if (!await _students.CheckForStudentAsync(user.Id, Context.Guild.Id))
                {
                    await ReplyAsync("They don't have any student profile! please make them use the verif command");
                    return;
                }

                Student student = await _students.GetStudentAsync(user.Id, Context.Guild.Id);
                string desc = null;

                if (student.Club != null)
                {
                    foreach (Club club in student.Club)
                    {
                        SocketRole role = Context.Guild.GetRole(club.RoleId);

                        if (role == null) { desc += $"{club.Name}, "; }

                        else { desc += $"{role.Mention}, "; }
                    }
                }

                builder.WithTitle($"{user.Username}'s Student profile");
                builder.WithFooter(student.UserId.ToString(), Context.Guild.IconUrl);
                builder.ThumbnailUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
                builder.AddField("Name", student.Name, false);
                builder.AddField("Surname", student.Surname, false);
                builder.AddField("Grade", student.Grade, false);
                builder.AddField("Clubs", desc ?? "none", false);
                
            }

            builder.WithCurrentTimestamp();
            Embed embed = builder.Build();

            await ReplyAsync(null, false, embed);

        }

        [Command("verification", RunMode = RunMode.Async), Alias("vérif", "v")]
        [Summary("creates a student profile")]
        public async Task Verif()
        {
            if (Context.Guild.GetChannel(812366359412473896) == null)
            {
                await ReplyAsync("Error channel doesn't exist");
                return;
            }

            if (await _students.CheckForStudentAsync(Context.User.Id, Context.Guild.Id))
            {
                await ReplyAsync("You already have a student profile!");
                return;
            }

            ITextChannel channel = (ITextChannel)Context.Guild.GetChannel(812366359412473896);

            Color color = (Color)Utilities.Utilities.RandomColor();

            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = color,
                Title = "**Enter your name**",
                ThumbnailUrl = Context.Guild.IconUrl,
            };
            Embed embed = builder.Build();
            await channel.SendMessageAsync(null, false, embed);
            
            SocketMessage Name = await NextMessageAsync(true, true, timeout: TimeSpan.FromMinutes(2));
            if (Name == null)
            {
                await channel.SendMessageAsync("You didn't reply in time, please retry by using the command again <a:Strokes:810926377737125958>");
                return;
            }

            builder.Title = "**Enter your surname**";
            embed = builder.Build();
            await channel.SendMessageAsync(null, false, embed);

            SocketMessage Surname = await NextMessageAsync(true, true, timeout: TimeSpan.FromMinutes(2));
            if (Surname == null)
            {
                await channel.SendMessageAsync("You didn't reply in time, please retry by using the command again <a:Strokes:810926377737125958>");
                return;
            }

            builder.Title = "**Enter your grade**";

            string desc = "Seconde/Première/Terminale";
            List<string> GradeT = new List<string>()
            {
                "Seconde",
                "Première",
                "Premiere",
                "Terminale"
            };

            builder.AddField("Grades", desc, false);
            embed = builder.Build();

            await channel.SendMessageAsync(null, false, embed);
            SocketMessage Grade = await NextMessageAsync(true, true, timeout: TimeSpan.FromMinutes(2));
            if (Grade == null)
            {
                await channel.SendMessageAsync("You didn't reply in time, please retry by using the command again <a:Strokes:810926377737125958>");
                return;

            }

            if (!GradeT.Contains(Grade.ToString()))
            {
                await channel.SendMessageAsync("You didn't answer correctly, please retry by using the command again <a:Strokes:810926377737125958>");
                return;
            }

            await _students.CreateStudentAsync(Context.User.Id, Context.Guild.Id, Name.ToString(), Surname.ToString(), Grade.ToString());
            if (Context.Guild.Id == 619525023300059136) { await (Context.User as IGuildUser).AddRoleAsync(Context.Guild.GetRole(620263935232311296)); }
            await channel.SendMessageAsync("Thank you, now you have access to the server!");
        }

        [Command("ForceVerif", RunMode = RunMode.Async), Alias("Fvérif", "fv")]
        [Summary("creates a student profile by asking them using the force")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task VerifMod(SocketGuildUser user = null)
        {
            if (user == null)
            {
                await ReplyAsync("Please precise an user");
                return;
            }

            if (await _students.CheckForStudentAsync(user.Id, Context.Guild.Id))
            {
                await ReplyAsync("You already have a student profile!");
                return;
            }

            if (Context.Guild.GetChannel(812366359412473896) == null)
            {
                await ReplyAsync("Error channel doesn't exist");
                return;
            }

            ITextChannel channel = (ITextChannel)Context.Guild.GetChannel(812366359412473896);

            Color color = (Color)Utilities.Utilities.RandomColor();

            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = color,
                Title = "**Enter your name**",
                ThumbnailUrl = Context.Guild.IconUrl,
            };
            Embed embed = builder.Build();
            await channel.SendMessageAsync(null, false, embed);

            SocketMessage Name = await NextMessageAsync(false, false, timeout: TimeSpan.FromMinutes(2));
            if (Name == null)
            {
                await channel.SendMessageAsync("You didn't reply in time, please retry by using the command again <a:Strokes:810926377737125958>");
                return;
            }

            builder.Title = "**Enter your surname**";
            embed = builder.Build();
            await channel.SendMessageAsync(null, false, embed);

            SocketMessage Surname = await NextMessageAsync(false, false, timeout: TimeSpan.FromMinutes(2));
            if (Surname == null)
            {
                await channel.SendMessageAsync("You didn't reply in time, please retry by using the command again <a:Strokes:810926377737125958>");
                return;
            }

            builder.Title = "**Enter your grade**";

            string desc = "Seconde/Première/Terminale";
            List<string> GradeT = new List<string>()
            {
                "Seconde",
                "Première",
                "Premiere",
                "Terminale"
            };

            builder.AddField("Grades", desc, false);
            embed = builder.Build();

            await channel.SendMessageAsync(null, false, embed);
            SocketMessage Grade = await NextMessageAsync(false, false, timeout: TimeSpan.FromMinutes(2));
            if (Grade == null)
            {
                await channel.SendMessageAsync("You didn't reply in time, please retry by using the command again <a:Strokes:810926377737125958>");
                return;
                
            }

            if (!GradeT.Contains(Grade.ToString()))
            {
                await channel.SendMessageAsync("You didn't answer correctly, please retry by using the command again <a:Strokes:810926377737125958>");
                return;
            }

            await _students.CreateStudentAsync(user.Id, Context.Guild.Id, Name.ToString(), Surname.ToString(), Grade.ToString());
            if (Context.Guild.Id == 619525023300059136) { await user.AddRoleAsync(Context.Guild.GetRole(620263935232311296)); }
            await channel.SendMessageAsync("Thank you, now you have access to the server!");
        }

        [Command("RemoveStudent"), Alias("rs", "removeS")]
        [Summary("removes a student profile using the force")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task RemoveStudent(SocketGuildUser user = null)
        {
            if (user == null)
            {
                await ReplyAsync("Please precise an user");
                return;
            }
            
            if(!await _students.CheckForStudentAsync(user.Id, Context.Guild.Id))
            {
                await ReplyAsync("The user doesn't have a profile");
                return;
            }

            if (Context.Guild.Id == 619525023300059136) { await user.RemoveRoleAsync(Context.Guild.GetRole(620263935232311296)); }
            await _students.RemoveStudentAsync(user.Id, Context.Guild.Id);
            await ReplyAsync($"{user.Mention}'s student profile has been erased");
        }

        [Command("AddClubStudent"), Alias("ACS", "AddCS", "AddClubS")]
        [Summary("Add a club to a student (with the role if it exists)")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task AddClubStudent(SocketGuildUser user, string clubName)
        {
            if (!await _clubs.CheckForClubAsync(clubName, Context.Guild.Id))
            {
                await ReplyAsync($"the club {clubName} doesn't exist!");
                return;
            }

            if (!await _students.CheckForStudentAsync(user.Id, Context.Guild.Id))
            {
                await ReplyAsync($"the student profile for {user.Mention} doesn't exist!");
                return;
            }
            Club club = await _clubs.GetClubAsync(clubName.ToLower(), Context.Guild.Id);
            Student student = await _students.GetStudentAsync(user.Id, Context.Guild.Id);

            if (student.Club.Contains(club))
            {
                await ReplyAsync($"This user already is in the {club.Name}!");
                return;
            }

            student.Club.Add(club);
            if (club.RoleId != 0 && Context.Guild.GetRole(club.RoleId) != null) { await user.AddRoleAsync(Context.Guild.GetRole(club.RoleId)); }
            await ReplyAsync($"The club {club.Name} has been added to the student profile of {user.Mention}");
        }

        [Command("RemoveClubStudent"), Alias("RCS", "RemoveCS", "RemoveClubS")]
        [Summary("Removes a club to a student (with the role if it exists)")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task RemoveClubStudent(SocketGuildUser user, string clubName)
        {
            if (!await _clubs.CheckForClubAsync(clubName, Context.Guild.Id))
            {
                await ReplyAsync($"the club {clubName} doesn't exist!");
                return;
            }

            if (!await _students.CheckForStudentAsync(user.Id, Context.Guild.Id))
            {
                await ReplyAsync($"the student profile for {user.Mention} doesn't exist!");
                return;
            }
            Club club = await _clubs.GetClubAsync(clubName.ToLower(), Context.Guild.Id);
            Student student = await _students.GetStudentAsync(user.Id, Context.Guild.Id);

            if (!student.Club.Contains(club))
            {
                await ReplyAsync($"This user isn't in the {club.Name}! What the hell is wrong with you...");
                return;
            }

            student.Club.Remove(club);
            if (club.RoleId != 0 && Context.Guild.GetRole(club.RoleId) != null) { await user.RemoveRoleAsync(Context.Guild.GetRole(club.RoleId)); }
            await ReplyAsync($"The club {club.Name} has been removed of the student profile of {user.Mention}");
        }

        [Command("RemoveAllClubsStudent"), Alias("RACS", "RemoveAllCS", "RemoveAllClubS")]
        [Summary("Removes all clubs to a student (with the role if it exists)")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task RemoveAllClubsStudent(SocketGuildUser user)
        {
            if (!await _students.CheckForStudentAsync(user.Id, Context.Guild.Id))
            {
                await ReplyAsync($"the student profile for {user.Mention} doesn't exist!");
                return;
            }

            await _students.ClearClubsStudentAsync(user.Id, Context.Guild.Id);
            await ReplyAsync($"The clubs from {user.Mention} student profile have been removed");
        }
    }
}
