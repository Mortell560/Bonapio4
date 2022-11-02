using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bonapio4Database
{
    public class Students
    {
        private readonly StudentContext _context;

        public Students(StudentContext context)
        {
            _context = context;
        }

        public async Task<Student> GetStudentAsync(ulong userId, ulong serverId)
        {
            Student student = await _context.Students
                .FindAsync(userId, serverId);

            return await Task.FromResult(student);
        }

        /// <summary>
        /// Creates a student and saves it into the database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="grade"></param>
        /// <returns>Nothing</returns>
        public async Task CreateStudentAsync(ulong userId, ulong serverId, string name, string surname, string grade)
        {
            Student student = await _context.Students
                .FindAsync(userId, serverId);

            if (student == null)
                _context.Add(new Student { UserId = userId, 
                    ServerId = serverId, 
                    Name = name, 
                    Surname = surname, 
                    Grade = grade,
                    Club = new List<Club>()});
            else
                return;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a student from the database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <returns>Nothing</returns>
        public async Task RemoveStudentAsync(ulong userId, ulong serverId)
        {
            Student student = await _context.Students
                .FindAsync(userId, serverId);

            if (student == null) { return; }

            _context.Remove(student);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds the club given as an argument to the student
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="club"></param>
        /// <returns>Nothing</returns>
        public async Task AddClubStudentAsync(ulong userId, ulong serverId, Club club)
        {
            Student student = await _context.Students
                .FindAsync(userId, serverId);

            if (student == null) { return; }

            student.Club.Add(club);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the club given as an argument to the student
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="club"></param>
        /// <returns>Nothing</returns>
        public async Task RemoveClubStudentAsync(ulong userId, ulong serverId, Club club)
        {
            Student student = await _context.Students
                .FindAsync(userId, serverId);

            if (student == null) { return; }

            student.Club.Remove(club);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Resets student club list back to bonapio
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <returns>Nothing</returns>
        public async Task ClearClubsStudentAsync(ulong userId, ulong serverId)
        {
            Student student = await _context.Students
                .FindAsync(userId, serverId);

            if (student == null) { return; }

            student.Club.Clear(); // user's list reset
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Check if a student exists
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <returns>bool</returns>
        public async Task<bool> CheckForStudentAsync(ulong userId, ulong serverId)
        {
            Student student = await _context.Students
                .FindAsync(userId, serverId);

            if (student == null)
                return false;
            else
                return true;
        }
    }
}
