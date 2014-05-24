using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace RS_Goal_Tracker.Data
{
    public class Highscores
    {
        public static string APIurl = "http://hiscore.runescape.com/index_lite.ws?player=";
        
        private static byte NameIndex = 0; 
        private static byte RankIndex = 1;
        private static byte LevlIndex = 2;
        private static byte ExppIndex = 3;        
        
        public static string LocalFile(string username)
        {
            return "hs_" + username + ".txt";
        }
        
        protected static bool downloadHighscores(string username)
        {
            Uri address = new Uri(APIurl + username);

            using (WebClient c = new WebClient())
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Polling highscores for " + username + " @ " + DateTime.Now.ToLongTimeString());
                    //System.Diagnostics.Debug.WriteLine(Environment.StackTrace);
                    c.DownloadFile(address, LocalFile(username));  
                    return true;
                }
                catch (WebException)
                {
                    System.Windows.Forms.MessageBox.Show("There was a problem accessing the server. Is the username correct?", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    //throw; // Shouldn't crash the app just because we couldn't grab new HS data
                    return false;
                }
            }
        }

        protected static List<Skill> parseHighscores(string username)
        {
            // Download highscores / check if already downloaded
            if(!File.Exists(LocalFile(username)))
            {
                try
                {
                    downloadHighscores(username);
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Can not create/access local highscores file.");
                    return null;
                }
            }

            // Build a list of string representations of each skill's attributes
            string[] rawLines = File.ReadAllLines(LocalFile(username));
            List<string[]> highscores = new List<string[]>();
            foreach(string line in rawLines)
            {
                List<string> breakLine = new List<string>(line.Trim().Split(','));
                // Skills have 3 listings, minigames 2. We're ignoring minigames.
                if (breakLine.Count < 3) continue; 
                // Insert the name of the skill at the front of the list
                breakLine.Insert(0, Skills.SkillList[highscores.Count].ToString());
                // Add the pieces to highscores listing
                highscores.Add(breakLine.ToArray());
            }

            // Build the proper Skill objects from the loaded text
            List<Skill> skillList = new List<Skill>();
            for (int i = 0; i < highscores.Count; i++)
            {
                Skills skillName = Skills.Parse(highscores[i][Highscores.NameIndex]);
                long skillExp = getSkillExp(highscores, skillName);
                int skillRank = getSkillRank(highscores, skillName);

                skillList.Add(new Skill(skillName, skillExp, skillRank));
            }

            return skillList;
        }

        protected static bool PurgeLocalFile(string username)
        {
            try
            {
                if (File.Exists(LocalFile(username)))
                    File.Delete(LocalFile(username));
                return true;
            }
            catch (IOException)
            {
                System.Diagnostics.Debug.WriteLine("Could not delete " + LocalFile(username));
                return false;
            }
        }

        public static bool Update(Player player)
        {
            int UpdateInterval = Properties.Settings.Default.MinUpdateInterval;
            bool outcome = true;
            
            Data.DatabaseInterface db = new Data.DatabaseInterface();
            List<Skill> skills;

            bool DatabaseOutofDate = DateTime.Now.Subtract(db.LastUpdate(player)).TotalMinutes >= UpdateInterval;

            if (DatabaseOutofDate)
                skills = parseHighscores(player.Username);
            else
                skills = db.GetSkillList(player.UserID);
            
            if (skills == null) outcome &= false;
            else player.UpdateCurrentSkills(skills);

            if(DatabaseOutofDate)
                db.AddPlayer(player); 

            outcome &= Highscores.PurgeLocalFile(player.Username); // delete the local copy of the file now that we're finished with it

            return outcome;
        }

        /// <summary>
        /// Creates a new Player object with data pulled from the Highscores website
        /// </summary>
        /// <param name="Username">Username of the player to search for.</param>
        /// <returns>Player object built from Highscores website. Returns null if download failed.</returns>
        public static Player Update(string Username)
        {
            Player fetchedPlayer = new Player(Username);

            if (Highscores.Update(fetchedPlayer))
                return fetchedPlayer;
            else
                return null;
        }

        private static long getValueAtIndex(List<string[]> highscores, string SkillName, int index)
        {
            foreach (string[] listing in highscores)
            {
                if (SkillName.ToLower().Equals(listing[NameIndex]))
                    return listing[index] == "" ? 0L : long.Parse(listing[index]);
            }

            return -1;
        }

        private static long getSkillExp(List<string[]> highscores, Skills SkillName)
        {
            return getValueAtIndex(highscores, SkillName.ToString(), ExppIndex);
        }

        private static int getSkillRank(List<string[]> highscores, Skills SkillName)
        {
            return (int)getValueAtIndex(highscores, SkillName.ToString(), RankIndex);
        }
    }
}
