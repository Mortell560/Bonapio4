using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bonapio4Database
{
    public class Clubs
    {
        private readonly StudentContext _context;

        public Clubs(StudentContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a Club using its name and serverId and returns it
        /// </summary>
        /// <param name="clubName"></param>
        /// <param name="serverId"></param>
        /// <returns>Club (club)</returns>
        public async Task<Club> GetClubAsync(string clubName, ulong serverId)
        {
            Club club = await _context.Clubs
                .FindAsync(clubName, serverId);

            return await Task.FromResult(club);
        }

        /// <summary>
        /// Removes a Club from the database
        /// </summary>
        /// <param name="clubName"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task RemoveClubAsync(string clubName, ulong serverId)
        {
            Club club = await _context.Clubs
                .FindAsync(clubName, serverId);

            if (club == null) { return; }

            _context.Remove(club);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a new club and saves it into the database
        /// </summary>
        /// <param name="clubName"></param>
        /// <param name="serverId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task CreateClubAsync(string clubName, ulong serverId)
        {
            Club club = await _context.Clubs
                .FindAsync(clubName, serverId);

            if (club == null)
                _context.Add(new Club { Name = clubName, ServerId = serverId});
            else
                return;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Check if a club exists
        /// </summary>
        /// <param name="clubName"></param>
        /// <param name="serverId"></param>
        /// <returns>bool</returns>
        public async Task<bool> CheckForClubAsync(string clubName, ulong serverId)
        {
            Club club = await _context.Clubs
                .FindAsync(clubName, serverId);

            if (club == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Get the list of all the clubs for a server
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns>List(Club)</returns>
        public async Task<List<Club>> GetClubsAsync(ulong serverId)
        {
            List<Club> club = new List<Club>();

            foreach (Club _club in _context.Clubs)
            {
                if (_club.ServerId == serverId) { club.Add(_club); }
            }

            return await Task.FromResult(club);
        }

        /// <summary>
        /// Set a roleId to a club in the database
        /// </summary>
        /// <param name="clubName"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task SetRoleClubAsync(string clubName, ulong serverId, ulong roleId)
        {
            Club club = await _context.Clubs
                .FindAsync(clubName, serverId);

            if (club == null) { return; }

            club.RoleId = roleId;

            _context.Attach(club);
            await _context.SaveChangesAsync();
        }
    }
}
