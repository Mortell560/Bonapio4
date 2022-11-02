using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bonapio4Database
{
    public class Profiles
    {
        private readonly StudentContext _context;

        public Profiles(StudentContext context)
        {
            _context = context;
        }

        public async Task<Profile> GetProfileAsync(ulong userId, ulong serverId)
        {
            Profile profile = await _context.Profiles
                .FindAsync(userId, serverId);

            return await Task.FromResult(profile);
        }

        /// <summary>
        /// Get all the profiles for a server
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task<List<Profile>> GetListProfileAsync(ulong serverId)
        {
            List<Profile> profiles = new List<Profile>();

            foreach (Profile _profile in _context.Profiles)
            {
                if (_profile.ServerId == serverId) { profiles.Add(_profile); }
            }

            return await Task.FromResult(profiles);
        }

        /// <summary>
        /// Creates a profile and saves it into the database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="grade"></param>
        /// <returns>Nothing</returns>
        public async Task CreateProfileAsync(ulong userId, ulong serverId)
        {
            Profile profile = await _context.Profiles
                .FindAsync(userId, serverId);

            if (profile == null)
                _context.Add(new Profile
                {
                    UserId = userId,
                    ServerId = serverId,
                    Xp = 0
                });
            else
                return;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a profile from the database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <returns>Nothing</returns>
        public async Task RemoveProfileAsync(ulong userId, ulong serverId)
        {
            Profile profile = await _context.Profiles
                .FindAsync(userId, serverId);

            if (profile == null) { return; }

            _context.Remove(profile);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Set the Xp to the amount given as an arg
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="xp"></param>
        /// <returns></returns>
        public async Task SetXpProfileAsync(ulong userId, ulong serverId, int xp)
        {
            Profile profile = await _context.Profiles
                .FindAsync(userId, serverId);

            if (profile == null) { return; }

            profile.Xp = xp;

            _context.Attach(profile);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Reset the Xp of the user back to 0
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task ResetXpProfileAsync(ulong userId, ulong serverId)
        {
            Profile profile = await _context.Profiles
                .FindAsync(userId, serverId);

            if (profile == null) { return; }

            profile.Xp = 0;

            _context.Attach(profile);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Add the amount of Xp to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="xp"></param>
        /// <returns></returns>
        public async Task AddXpProfileAsync(ulong userId, ulong serverId, int xp)
        {
            Profile profile = await _context.Profiles
                .FindAsync(userId, serverId);

            if (profile == null) { return; }

            profile.Xp += xp;

            _context.Attach(profile);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Check if a profile exists
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <returns>bool</returns>
        public async Task<bool> CheckForProfileAsync(ulong userId, ulong serverId)
        {
            Profile profile = await _context.Profiles
                .FindAsync(userId, serverId);

            if (profile == null)
                return false;
            else
                return true;
        }
    }
}
