using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using RS_Goal_Tracker.Data;

using System.Diagnostics;

namespace RS_Goal_Tracker.Data
{
    [Obsolete]
    public class DatabaseInterface
    {
        protected string ConnectionString
        {
            get { return Properties.Settings.Default.LocalScoresConnectionString; }
        }

        // TODO DatabaseInterface needs a re-write using LINQ queries
        // I got about 90% of the way through writing this class up
        // before I realized I could have just fucking used LINQ to SQL.
        //
        // I don't have the willpower to rewrite this just yet...
        // That's what I get for working on this at 4am.

        public DatabaseInterface()
        {
            // Create new database file if not present
            if (!DatabaseExists())
                CreateNewDatabase();

            // THOUGHT if the database gets removed/corrupted/etc between when the constructor is called -
            //         and when a method (such as GetUsernames) is called, then we're fucked.
        }

        protected void CreateNewDatabase()
        {
            throw new NotImplementedException("CreateNewDatabase has not been fully implemented.");

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                string CreateDBQuery = " CREATE DATABASE ON PRIMARY ";

                using (SqlCommand cmd = new SqlCommand(CreateDBQuery, connection))
                {
                    try
                    {
                        connection.Open();
                        Debug.WriteLine(CreateDBQuery);

                        cmd.ExecuteNonQuery();
                        Debug.WriteLine("The new database was created successfully.");
                    }
                    catch(SqlException)
                    {
                        System.Windows.Forms.MessageBox.Show("There was a problem creating the database.\n" + 
                            "It would be very helpful if you could send a bug report to bugs@hepati.ca", 
                            "Fetal Error", System.Windows.Forms.MessageBoxButtons.OK);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        protected bool DatabaseExists()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine("Database file could not be found.");
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);

                this.CreateNewDatabase();

                string message = "The LocalScores database was missing.\n" +
                                 "If you didn't delete the database (purposefully) there might be a problem.\n\n" +
                                 "I'll try to create a new database, but any data you may have collected is gone.";

                System.Windows.Forms.MessageBox.Show(message, "Alert", System.Windows.Forms.MessageBoxButtons.OK);
                return false;
            }

            return true; 
        }

        /// <summary>
        /// Queries the Users table of the database to get a list of Usernames stored within.
        /// </summary>
        /// <returns>A linked list of all Usernames (string) stored in the database.</returns>
        public List<string> GetUsernames()
        {
            // HACK... This is shit. Must be a cleaner way to accomplish the same thing,
            // while still avoiding too much redundant code.
            return this.ReadUsers(false).Cast<string>().ToList(); 
        }

        /// <summary>
        /// Queries the Users table of the database to get a list of UserIDs.
        /// </summary>
        /// <returns>A linked list of all UserIDs (Guid) stored in the database.</returns>
        public List<Guid> GetUserIDs()
        {
            return this.ReadUsers(true).Cast<Guid>().ToList();
        }

        private List<object> ReadUsers(bool byUserID)
        {
            List<object> usersList = new List<object>();

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users", connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            usersList.Add((byUserID ? reader["UserID"] : reader["Username"]));
                        }
                    } 
                }

                connection.Close();
            }

            return usersList;
        }

        /// <summary>
        /// Retrieves the latest entries of all skills for Player with matching UserID
        /// </summary>
        /// <param name="UserID">UserID of the Player to search for.</param>
        /// <returns>New Player object with latest data from the database.</returns>
        public Player GetPlayer(Guid UserID)
        {
            Player newPlayer = null;

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users", connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Guid readID = (Guid)reader["UserID"];
                            if (readID.Equals(UserID))
                            {
                                newPlayer = new Player((string)reader["Username"]);
                                break;
                            }
                        }
                    }
                }
                connection.Close();
            }

            if (newPlayer != null)
                newPlayer.UpdateCurrentSkills(this.GetSkills(newPlayer));
            else
                Debug.WriteLine("Could not load data for Player " + UserID.ToString() + ". May not be in the database.");

            return newPlayer;
        }

        private List<Skill> GetSkills(Player player)
        {
            List<Skill> skills = new List<Skill>();

            using(SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT * FROM Scores WHERE Timestamp = (" + 
                    "SELECT MAX(Timestamp) FROM Scores WHERE [UserID] = @UserID)", connection))
                {
                    cmd.Parameters.Add("@UserID", System.Data.SqlDbType.UniqueIdentifier, 16).Value = player.UserID;

                    SqlDataReader reader = cmd.ExecuteReader();
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            skills.Add(new Skill(
                                Skills.Parse(reader["Skillname"]),
                                (long)reader["Experience"],
                                (int)reader["Rank"]));
                        }
                    }
                }
                connection.Close();
            }

            return skills;
        }

        /// <summary>
        /// Queries the database to find the most recent score data for matching skillname and UserID.
        /// If you want to retieve more than one Skill's data, use GetPlayer() instead.
        /// </summary>
        public Skill GetSkill(Skills skillname, Guid UserID)
        {
            Skill skill = null; // null object is returned if no matching UserID is in the database
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Scores WHERE Timestamp = " +
                    "(SELECT MAX(Timestamp) FROM Scores WHERE [UserID] = @UserID AND [Skillname] = @Skillname)", connection))
                {
                    cmd.Parameters.Add("@UserID", System.Data.SqlDbType.UniqueIdentifier, 16).Value = UserID;
                    cmd.Parameters.Add("@Skillname", System.Data.SqlDbType.NVarChar).Value = skillname.ToString();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            skill = new Skill(Skills.Parse(reader["Skillname"]), (long)reader["Experience"], (int)reader["Rank"]);
                        }
                    }
                }
                connection.Close();
            }
            
            return skill;
        }

        /// <summary>
        /// Pulls all matching Skill objects from the database from a specific time range.
        /// </summary>
        /// <param name="skillname">Skills object to match for which skill to select.</param>
        /// <param name="UserID">UserID for the player whose skill data is to be selected.</param>
        /// <param name="fromDate">DateTime object representing the beginning of the time range to select from.</param>
        /// <param name="toDate">DateTime object representing the end of the time range to select from.</param>
        /// <returns></returns>
        public List<Skill> GetSkillRange(Skills skillname, Guid UserID, DateTime fromDate, DateTime toDate)
        {
            List<Skill> SkillRange = new List<Skill>();

            using(SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Scores WHERE Timestamp BETWEEN @FromDate and @ToDate AND " +
                    "[UserID] = @UserID AND [Skillname] = @Skillname", connection))
                {
                    cmd.Parameters.Add("@FromDate", System.Data.SqlDbType.DateTime, 8).Value = fromDate;
                    cmd.Parameters.Add("@ToDate", System.Data.SqlDbType.DateTime, 8).Value = toDate;
                    cmd.Parameters.Add("@UserID", System.Data.SqlDbType.UniqueIdentifier, 16).Value = UserID;
                    cmd.Parameters.Add("@Skillname", System.Data.SqlDbType.NVarChar).Value = skillname.ToString();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            //Debug.WriteLine(reader["Skillname"] + ": " + reader["Experience"] + " @ " + ((DateTime)reader["Timestamp"]).ToLongDateString());
                            SkillRange.Add(new Skill(Skills.Parse(reader["Skillname"]), (long)reader["Experience"], (int)reader["Rank"]));
                        }
                    }
                }
                connection.Close();
            }

            return SkillRange;
        }

        /// <summary>
        /// DEBUG PURPOSES ONLY. Adds a player, and it's skills to the database with a custom (dummy) timestamp.
        /// </summary>
        private void Debug_InsertDummy(Player player, DateTime timestamp)
        {
            this.InsertUserID(player);

            #region Code from this.Update() modified to use custom (dummy) timestamp
            if (!PlayerInDatabase(player))
                this.InsertUserID(player);

            // Build a SqlCommand string to insert all of player.Skills' attributes into the Scores table
            StringBuilder SqlCommandString = new StringBuilder("INSERT INTO Scores (UserID, Skillname, Experience, Rank, Timestamp) VALUES ");
            var rows = new List<Tuple<Guid, string, long, int, DateTime>>();
            foreach (Skill skill in player.Skills)
            {
                int count = rows.Count();
                if (count != 0) // the first entry doesn't need the comma
                    SqlCommandString.Append(",");

                SqlCommandString.AppendFormat("(@UserID{0}, @Skillname{0}, @Experience{0}, @Rank{0}, @Timestamp{0})", count);
                rows.Add(new Tuple<Guid, string, long, int, DateTime>(
                    player.UserID,
                    skill.SkillsName.ToString(),
                    skill.CurrentExperience,
                    skill.Rank,
                    timestamp));
            }

            if (rows.Count > 0)
            {
                using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(SqlCommandString.ToString(), connection, transaction))
                        {
                            for (int i = 0; i < rows.Count; i++)
                            {
                                cmd.Parameters.Add("@UserID" + i, System.Data.SqlDbType.UniqueIdentifier, 16).Value = rows[i].Item1;
                                cmd.Parameters.Add("@Skillname" + i, System.Data.SqlDbType.NVarChar).Value = rows[i].Item2;
                                cmd.Parameters.Add("@Experience" + i, System.Data.SqlDbType.BigInt).Value = rows[i].Item3;
                                cmd.Parameters.Add("@Rank" + i, System.Data.SqlDbType.Int).Value = rows[i].Item4;
                                cmd.Parameters.Add("@Timestamp" + i, System.Data.SqlDbType.DateTime2, 8).Value = rows[i].Item5;
                            }
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                    }
                    connection.Close();
                }
            }
            #endregion
        }

        /// <summary>
        /// Inserts the Player into the database. 
        /// </summary>
        /// <param name="player">New player to add to the 'Users' table.</param>
        /// <param name="includeStats">If true all of this Player's stats will be added to the 'Scores' table.</param>
        private void InsertUserID(Player player)
        {
            if(PlayerInDatabase(player))
                return; // We don't need to insert a new player if it already exists in the database.

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (UserID, Username) VALUES (@UserID, @Username)"))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.Add("@UserID", System.Data.SqlDbType.UniqueIdentifier, 16).Value = player.UserID;
                    cmd.Parameters.AddWithValue("@Username", player.Username);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Inserts one Skill object's data for 'player' into the database. 
        /// If you need to insert more than one skill at a time, use Insert(Player) instead.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="player"></param>
        protected void Insert(Skill skill, Player player)
        {
            if (!PlayerInDatabase(player))
                this.InsertUserID(player);

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Scores (UserID, Skillname, Experience, Rank, Timestamp) " +
                    "VALUES (@UserID, @Skillname, @Experience, @Rank, @Timestamp)"))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;

                    cmd.Parameters.Add("@UserID", System.Data.SqlDbType.UniqueIdentifier, 16).Value = player.UserID;
                    cmd.Parameters.Add("@Skillname", System.Data.SqlDbType.NVarChar).Value = skill.SkillsName.ToString();
                    cmd.Parameters.Add("@Experience", System.Data.SqlDbType.BigInt).Value = skill.CurrentExperience;
                    cmd.Parameters.Add("@Rank", System.Data.SqlDbType.Int).Value = skill.Rank;
                    cmd.Parameters.Add("@Timestamp", System.Data.SqlDbType.DateTime2, 8).Value = DateTime.Now;

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Inserts new entries for all of Player.Skills into the 'Scores' table
        /// </summary>
        /// <param name="player">Player object whose skills are being updated.</param>
        public void Insert(Player player)
        {
            if (!PlayerInDatabase(player))
                this.InsertUserID(player);

            // Ensure consistent Timestamps for all Skills in this update
            DateTime timestamp = DateTime.Now;

            // Build a SqlCommand string to insert all of player.Skills' attributes into the Scores table
            StringBuilder SqlCommandString = new StringBuilder("INSERT INTO Scores (UserID, Skillname, Experience, Rank, Timestamp) VALUES ");
            var rows = new List<Tuple<Guid, string, long, int, DateTime>>();
            foreach (Skill skill in player.Skills)
            {
                int count = rows.Count();
                if (count != 0) // the first entry doesn't need the comma
                    SqlCommandString.Append(",");

                SqlCommandString.AppendFormat("(@UserID{0}, @Skillname{0}, @Experience{0}, @Rank{0}, @Timestamp{0})", count);
                rows.Add(new Tuple<Guid, string, long, int, DateTime>(
                    player.UserID,
                    skill.SkillsName.ToString(),
                    skill.CurrentExperience,
                    skill.Rank,
                    timestamp));
            }

            if (rows.Count > 0)
            {
                using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(SqlCommandString.ToString(), connection, transaction))
                        {
                            for(int i = 0; i < rows.Count; i++)
                            {
                                cmd.Parameters.Add("@UserID" + i, System.Data.SqlDbType.UniqueIdentifier, 16).Value = rows[i].Item1;
                                cmd.Parameters.Add("@Skillname" + i, System.Data.SqlDbType.NVarChar).Value = rows[i].Item2;
                                cmd.Parameters.Add("@Experience" + i, System.Data.SqlDbType.BigInt).Value = rows[i].Item3;
                                cmd.Parameters.Add("@Rank" + i, System.Data.SqlDbType.Int).Value = rows[i].Item4;
                                cmd.Parameters.Add("@Timestamp" + i, System.Data.SqlDbType.DateTime2, 8).Value = rows[i].Item5;
                            }
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch(SqlException e)
                    {
                        transaction.Rollback();
                        Debug.WriteLine(e.Message);
                    }
                    connection.Close();
                }
            }
        }

        public bool PlayerInDatabase(Player player)
        {
            return this.GetUserIDs().Contains(player.UserID);

            /* 
            List<Guid> IDList = this.GetUserIDs();
            foreach(Guid userID in IDList)
                if (player.UserID.Equals(userID))
                    return true;

            return false;
             */
        }

        public DateTime LastUpdate(Player player)
        {
            // TODO query the db to see when the last update was performed for player
            throw new NotImplementedException("LastUpdate() is not yet implemented.");
        }

        public static void TestHarness()
        {
            #region Test Case 1
            /*
            Player p1 = new Player("p atrick");
            p1.DownloadHighscores();

            Player p2 = new Player("kelleta");
            p2.DownloadHighscores();

            DatabaseInterface db = new DatabaseInterface();

            #region test insert p1
            db.Insert(p1, true);

            Debug.WriteLine("Users in database:");
            foreach (string username in db.GetUsernames())
                Debug.Write(username + ", ");

            Debug.WriteLine("");
            #endregion

            #region test insert p2
            db.Insert(p2, true);

            Debug.WriteLine("Users in database:");
            foreach (string username in db.GetUsernames())
                Debug.Write(username + ", ");

            Debug.WriteLine("");
            #endregion

            #region test getSkills p1
            Debug.WriteLine("\np1 skills: ");
            foreach (Skill s in db.GetSkills(p1))
                Debug.WriteLine(s.ToString());
            Debug.WriteLine("");
            #endregion

            #region test getSkills p2
            Debug.WriteLine("\np2 skills: ");
            foreach (Skill s in db.GetSkills(p2))
                Debug.WriteLine(s.ToString());
            Debug.WriteLine("");
            #endregion

            #region test update p1
            Skill nSkill = new Skill(Skills.Attack, 666666, 42);
            p1.UpdateSkill(nSkill, false);

            db.Update(p1);

            Debug.WriteLine("\np1 (updated) skills: ");
            foreach (Skill s in db.GetSkills(p1))
                Debug.WriteLine(s.ToString());
            Debug.WriteLine("");
            #endregion

            #region test update p1
            Skill nSkill2 = new Skill(Skills.Smithing, 666666, 42);
            p2.UpdateSkill(nSkill2, false);

            db.Update(p2);

            Debug.WriteLine("\np2 (updated) skills: ");
            foreach (Skill s in db.GetSkills(p2))
                Debug.WriteLine(s.ToString());
            Debug.WriteLine("");
            #endregion
            */
            #endregion
        }

        public static string TestDBDump()
        {
            StringBuilder s = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\LocalScores.mdf;Integrated Security=True"))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Scores", connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            s.AppendFormat("{0} - {1} @ {2}: {3}, {4}, {5} ", "()", reader["UserID"], ((DateTime)reader["Timestamp"]).ToLongTimeString(), reader["Skillname"], reader["Experience"], reader["Rank"]);
                            s.AppendLine("");
                        }
                    }
                    else
                    {
                        s.Append("Reader was empty.");
                    }
                }
                connection.Close();
            }

            return s.ToString();
        }

        public static string TestLoad()
        {
            DatabaseInterface db = new DatabaseInterface();
            Player p1 = new Player("p atrick");

            StringBuilder str = new StringBuilder();

            str.Append("\np1 skills: ");
            foreach (Skill s in db.GetSkills(p1))
                str.Append(s.ToString());
            str.Append("");

            return str.ToString();
        }

        public static string TestReadRange()
        {
            List<Player> dummyPlayers = new List<Player>();
            
            DatabaseInterface db = new DatabaseInterface();

            // create some fake players
            for (int i = 0; i <= 5; i++ )
            {
                Player dummy = new Player("bob" + i.ToString());
                dummy.DEBUG_DummyFill();

                dummyPlayers.Add(dummy);
            }

            // fill db up with a bunch of bullshit
            for (int i = 0; i < 25; i++)
            {
                Skill dummySkill = new Skill(Skills.Runecrafting, 16000000L + (128000 * i), 6000 - i);
                dummyPlayers[1].UpdateSkill(dummySkill, false, true);

                db.Debug_InsertDummy(dummyPlayers[1], DateTime.Now.AddDays(i));
            }

            StringBuilder str = new StringBuilder();

            foreach(Skill s in db.GetSkillRange(Skills.Runecrafting, dummyPlayers[1].UserID, DateTime.Now.AddDays(5), DateTime.Now.AddDays(42)))
            {
                str.AppendLine(s.ToString());
            }

            return str.ToString();
        }
    }
}
