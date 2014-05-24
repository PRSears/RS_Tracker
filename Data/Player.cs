using System;
using System.Text;
using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace RS_Goal_Tracker.Data
{
    [Table(Name = "Users")]
    public class Player
    {
        [Column]
        public string Username
        {
            get;
            set;
        }

        private Guid _UserID;
        [Column(IsPrimaryKey=true, Storage="_UserID")]
        public Guid UserID
        {
            get
            {
                if (_UserID == null || _UserID.Equals(Guid.Empty))
                    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                        this._UserID = new Guid(md5.ComputeHash(Encoding.Default.GetBytes(this.Username)));

                return this._UserID;
            }
            protected set
            {
                this._UserID = value;
            }
        }
        
        public List<Skill> Skills
        {
            get;
            private set;
        }

        public Player(string username)
        {
            this.Skills = new List<Skill>();

            // Populate the list of skills with default Skill objects
            foreach(Skills skill in Data.Skills.SkillList)
                Skills.Add(new Skill(skill));

            Username = username.ToLower();
            UserID = UserID; // HACK forces Guid to get calculated
        }

        public Player()
        {
            this.Username = String.Empty;
            this.Skills = new List<Skill>();
            
            // Populate the list of skills with default Skill objects
            foreach (Skills skill in Data.Skills.SkillList)
                Skills.Add(new Skill(skill));
        }

        /// <summary>
        /// Searches for and returns a matching Skills (skill name) object from this Player's skill list.
        /// </summary>
        /// <param name="skill">Skills object to retrieve.</param>
        /// <returns>A Skill object of the matching skill from this Player.</returns>
        public Skill GetSkill(Skills skill)
        {
            foreach(Skill playerSkill in this.Skills)
                if (playerSkill.SkillsName.Equals(skill))
                    return playerSkill;

            return null; 
        }

        /// <summary>
        /// Updates this Player's matching Skill object with the values from "skill".
        /// </summary>
        /// <param name="skill">Skill object with values we want to update to.</param>
        /// <param name="updateGoals">If true any goal information contained in "skill" will overwrite in Player's matching Skill.</param>
        /// <param name="updateCurrents">If true any Experience and Rank information contained in 'skill' will overwrite in Player's matching Skill.</param>
        public Player UpdateSkill(Skill skill, bool updateGoals, bool updateCurrents)
        {
            Skill skillToUpdate = this.GetSkill(skill.SkillsName);

            if (updateCurrents) skillToUpdate.CurrentExperience = skill.CurrentExperience;
            if (updateCurrents) skillToUpdate.Rank = skill.Rank;
            if (updateGoals) skillToUpdate.GoalExperience = skill.GoalExperience;

            return this;
        }

        /// <summary>
        /// Update current skill data for all entries in Player.Skills while preserving any goal information for each skill.
        /// </summary>
        /// <param name="skills">List of Skill objects whose CurrentExperience and Rank we want to update Player's Skills with.</param>
        public Player UpdateCurrentSkills(List<Skill> skills)
        {
            foreach(Skill skill in skills)
            {
                this.UpdateSkill(skill, false, true);
            }

            return this;
        }

        /// <summary>
        /// Update goal data for all entries in Player.Skills while preserving current experience and rank.
        /// </summary>
        /// <param name="goals">List of Skill objects whose goals we want to update Player's Skills with.</param>
        /// <returns></returns>
        public Player UpdateGoals(List<Skill> goals)
        {
            foreach(Skill skill in goals)
            {
                this.UpdateSkill(skill, true, false);
            }

            return this;
        }
        
        public override string ToString()
        {
            return this.Username + " [" + this.UserID.ToString() + "]";
        }

        public void DEBUG_DummyFill()
        {            
            foreach(Data.Skills s in Data.Skills.SkillList)
            {
                Random r = new Random();
                Skill dummy = new Skill(s, r.Next(123456, 123456789), r.Next(1000, 9999));
                this.UpdateSkill(dummy, false, true);
            }
        }
    }
}
