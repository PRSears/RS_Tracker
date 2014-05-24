using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


namespace RS_Goal_Tracker.Data
{
    public class DatabaseInterface
    {
        private static bool REPLACE = true;

        public string ConnectionString
        { 
            get 
            {
                string connection = Properties.Settings.Default.LocalScoresConnectionString;

                if (connection.Contains("|DataDirectory|") && REPLACE)
                    connection = connection.Replace("|DataDirectory|", Directory.GetCurrentDirectory());
                
                return connection; 
            }
        }

        public List<string> Usernames
        {
            get
            {
                using (HighscoresDataContext context = new HighscoresDataContext(this.ConnectionString))
                {
                    var names = from users in context.Users
                                select users.Username;

                    return names.ToList();
                }
            }
        }

        public List<Guid> UserIDs
        {
            get
            {
                using (HighscoresDataContext context = new HighscoresDataContext(this.ConnectionString))
                {
                    var IDs = from users in context.Users
                              select users.UserID;

                    return IDs.ToList();
                }
            }
        }

        public DatabaseInterface()
        {
            using (HighscoresDataContext context = new HighscoresDataContext(this.ConnectionString))
            {
                if(!context.DatabaseExists())
                {
                    try
                    {
                        context.CreateDatabase();
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show("Database is missing. If you deleted it, you're pretty much fucked.\nYou'll also need to delete the local instance before a new database can be created.", "Missing Databse.");
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if player exists in the database.
        /// </summary>
        public bool PlayerInDatabase(Player player) { return this.UserIDs.Contains(player.UserID); }

        /// <summary>
        /// Retrieves the latest entries of all skills for Player with matching UserID.
        /// </summary>
        /// <param name="UserID">UserID of the Player to search for.</param>
        /// <returns>New Player object with latest data from the database.</returns>
        public Player GetPlayer(Guid UserID)
        {
            Player retrievedPlayer = null;
            
            using (HighscoresDataContext context = new HighscoresDataContext(this.ConnectionString))
            {
                retrievedPlayer = context.Users.Single(s => s.UserID.Equals(UserID));
                retrievedPlayer.UpdateCurrentSkills(this.GetSkillList(UserID));
            }

            return retrievedPlayer;
        }

        /// <summary>
        /// Retrieves the latest Skill objects from the database with matching UserID.
        /// </summary>
        /// <param name="UserID">UserID to retrieve Skill objects for.</param>
        /// <returns>A List of all recent Skill objects with matching UserID.</returns>
        public List<Skill> GetSkillList(Guid UserID)
        {
            using(HighscoresDataContext context = new HighscoresDataContext(this.ConnectionString))
            {
                var retrievedSkills = context.Scores
                                                .Where(s => s.UserID == UserID)
                                                .GroupBy(
                                                    g => g.Skillname,
                                                    (key, group) => group.Single(
                                                        s => s.Timestamp == group.Max(t => t.Timestamp)));
                return retrievedSkills.ToList();
            }
        }

        /// <summary>
        /// Pulls all matching Skill objects from the database from a specific time range.
        /// </summary>
        /// <param name="skillname">Skills object to match for which skill to select.</param>
        /// <param name="UserID">UserID for the player whose skill data is to be selected.</param>
        /// <param name="fromDate">DateTime object representing the beginning of the time range to select from.</param>
        /// <param name="toDate">DateTime object representing the end of the time range to select from.</param>
        /// <returns>Returns a List of all Skill objects which were added to the database within the specified time range.</returns>
        public List<Skill> GetSkillRange(Skills Skillname, Guid UserID, DateTime fromDate, DateTime toDate)
        {
            using (HighscoresDataContext context = new HighscoresDataContext(this.ConnectionString))
            {
                var range = context.Scores.Where(skill => skill.UserID == UserID && skill.Timestamp >= fromDate && skill.Timestamp <= toDate)
                                          .GroupBy(skill => skill.Skillname)
                                          .First(group => group.Key == Skillname.ToString());

                return range.ToList();
            }
        }

        /// <summary>
        /// Adds new entries for all of Player.Skills into the 'Scores' table, and adds player to 'Users' table if necessary.
        /// </summary>
        /// <param name="player">Player object whose skills are added.</param>
        public void AddPlayer(Player player)
        {
            using(HighscoresDataContext context = new HighscoresDataContext(this.ConnectionString))
            {
                if(!PlayerInDatabase(player))
                    context.Users.InsertOnSubmit(player);

                DateTime timestamp = DateTime.Now;

                foreach(Skill skill in player.Skills)
                {
                    if (!(skill.Equals(Skill.Empty) || skill.SkillsName.Equals(Skills.Placeholder)))
                    {
                        skill.Timestamp = timestamp;
                        skill.UserID = player.UserID;
                        skill.UniqueID = new Guid(skill.GetHashCode());

                        context.Scores.InsertOnSubmit(skill);
                    }
                }

                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Finds the date and time of most recent update for player.
        /// </summary>
        /// <param name="player">Player with UserID to query the database with.</param>
        /// <returns>Most recent DateTime Timestamp in 'Skills' table for player.</returns>
        public DateTime LastUpdate(Player player)
        {
            using(HighscoresDataContext context = new HighscoresDataContext(this.ConnectionString))
            {
                if (!PlayerInDatabase(player)) return DateTime.MinValue;

                var time = context.Scores.Where(skill => skill.UserID == player.UserID)
                                         .Max(t => t.Timestamp);
                return time;
            }
        }
    }
}
