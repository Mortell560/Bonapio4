using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bonapio4Database
{
    public class Servers
    {
        private readonly StudentContext _context;

        public Servers(StudentContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Modifies Guild's prefix. If the server wasn't in the database then the prefix and serverId are directly saved into the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task ModifyGuildPrefix(ulong id, string prefix)
        {
            Server server = await _context.Servers
                .FindAsync(id);

            if (server == null)
                _context.Add(new Server { Id = id, Prefix = prefix });
            else
                server.Prefix = prefix;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Returns given guild's prefix
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string (Guild's prefix)</returns>
        public async Task<string> GetGuildPrefix(ulong id)
        {
            string prefix = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.Prefix)
                .FirstOrDefaultAsync();

            return await Task.FromResult(prefix);
        }

        /// <summary>
        /// Changes the log channel of a server to the one given as an args
        /// </summary>
        /// <param name="id"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public async Task ModifyLogsAsync(ulong id, ulong channelId)
        {
            Server server = await _context.Servers
                .FindAsync(id);

            if (server == null)
                _context.Add(new Server { Id = id, Logs = channelId });
            else
                server.Logs = channelId;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Sets to 0 the log channelId which means that it has been deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveLogsAsync(ulong id)
        {
            Server server = await _context.Servers
                .FindAsync(id);

            server.Logs = 0;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets log channel id using the server Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ulong (LogChannelId)</returns>
        public async Task<ulong> GetLogsAsync(ulong id)
        {
            Server server = await _context.Servers
                .FindAsync(id);

            return await Task.FromResult(server.Logs);
        }
        /// <summary>
        /// Changes the spam channel of a server to the one given as an args
        /// </summary>
        /// <param name="id"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public async Task ModifySpamAsync(ulong id, ulong channelId)
        {
            Server server = await _context.Servers
                .FindAsync(id);

            if (server == null)
                _context.Add(new Server { Id = id, Spam = channelId });
            else
                server.Spam = channelId;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Sets to 0 the Spam channelId which means that it has been deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveSpamAsync(ulong id)
        {
            Server server = await _context.Servers
                .FindAsync(id);

            server.Spam = 0;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets Spam channel id using the server Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ulong (LogChannelId)</returns>
        public async Task<ulong> GetSpamAsync(ulong id)
        {
            Server server = await _context.Servers
                .FindAsync(id);

            if (server == null) { return 0; }

            return await Task.FromResult(server.Spam);
        }

        public async Task CreateServerAsync(ulong id)
        {
            _context.Add(new Server { Id = id });
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if the server is already in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        public async Task<bool> CheckForServerAsync(ulong id)
        {
            Server server = await _context.Servers
                .FindAsync(id);

            if (server == null) { return false; }

            return true;
        }

    }
}
