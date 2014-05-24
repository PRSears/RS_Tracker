using System;
using System.Text;
using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace RS_Goal_Tracker.Data
{
    [Table(Name="Scores")]
    public class Skill
    {
        [Column(IsPrimaryKey = true)]
        public Guid UniqueID;
        [Column]
        public Guid UserID;
        [Column]
        public DateTime Timestamp;
        [Column]
        public string Skillname;
        public Skills SkillsName
        {
            get { return Skills.Parse(this.Skillname); }
        }
        private int currentLevel;
        public int CurrentLevel
        {
            get { return currentLevel; }
            set
            {
                if(value < 0) throw new ArgumentOutOfRangeException();

                currentLevel = value;
                currentExperience = ExperienceAtLevel(CurrentLevel);
            }
        }
        private long currentExperience;
        [Column(Storage="currentExperience")]
        public long CurrentExperience
        {
            get { return currentExperience; }
            set
            {
                currentExperience = value >= 0 ? value : 0;
                currentLevel = LevelAtExperience(currentExperience);
            }
        }
        private int goalLevel;
        public int GoalLevel
        {
            get { return goalLevel; }
            set 
            {
                if (value < 0) throw new ArgumentOutOfRangeException();

                goalLevel = value;
                goalExperience = ExperienceAtLevel(goalLevel);
            }
        }
        private long goalExperience;
        public long GoalExperience
        {
            get { return goalExperience; }
            set
            {
                goalExperience = value >= 0 ? value : 0;
                goalLevel = LevelAtExperience(goalExperience);
            }
        }
        [Column]
        public int Rank
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum possible experience points the game will allow for a skill.
        /// </summary>
        public static long MaxSkillXP = 6000000000L;
        /// <summary>
        /// The maximum (theoretically) possible level at MaxXP.
        /// </summary>
        public static int MaxLevel = 126;

        public Skill(Skills skillName, int startLevel = 1, long startExperience = 0L, int endLevel = 99, long endExperience = 13034431L, int rank = -1)
        {
            Skillname = skillName.ToString();
            Rank = rank;
            
            // If a value has been passed for startLevel, and not startExperience we take the level and calculate experience
            if(startLevel != 1 && startExperience == 0)
            {
                currentLevel = startLevel;
                currentExperience = ExperienceAtLevel(currentLevel);
            }
            else if(startExperience != 0 && startLevel == 1) // if we have a value for experience, but not level...
            {
                currentExperience = startExperience;
                currentLevel = LevelAtExperience(currentExperience);
            }

            // If a value has been passed for endLevel, and not endExperience we take the level and calculate experience
            if (endLevel != 1 && endExperience == 0)
            {
                goalLevel = endLevel;
                goalExperience = ExperienceAtLevel(endLevel);
            }
            else if (endExperience != 0 && endLevel == 1) // if we have a value for experience, but not level...
            {
                goalExperience = endExperience;
                goalLevel = LevelAtExperience(endExperience);
            }
        }

        /// <summary>
        /// Construct a new Skill object with default goal information. 
        /// </summary>
        /// <param name="skillName">Skills object specifying which still this is.</param>
        /// <param name="startExperience">Current player experience in this skill.</param>
        /// <param name="rank">Current player rank in this skill.</param>
        public Skill(Skills skillName, long startExperience, int rank):this(skillName, 1, startExperience, 99, 0, rank)
        {
        }

        public Skill(Skills skillName, long startExperience, int rank, long goalExperience):this(skillName, 1, startExperience, 1, goalExperience, rank)
        {
        }

        public Skill() : this(Skills.Total, 1, 0, 1, 0, -1) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static long ExperienceAtLevel(int level)
        {
            if (level < 1)
                return 0;

            // Thanks to SlowByte and Sir T Bone: http://rsdo.net/rsdonline/guides/Experience%20formula.html
            double p = 0;
            
            for (int x = 1; x < level; x++)
            {
                p += Math.Floor(x + 300 * Math.Pow(2, (x / 7.0)) );
            }

            return (long)Math.Floor(p / 4);
        }
       
        public static long ExperienceAtLevel(long level)
        {
            // Just did this so I didn't have to write (int)level over and over again.
            return ExperienceAtLevel((int)level);
        }

        public static int LevelAtExperience(long experience)
        {
            if (experience > MaxSkillXP) return 200; // It's probably Total XP
            if (experience < ExperienceAtLevel(2))
                return 1;
            
            for(long n = 2, nLevelExp = ExperienceAtLevel(n); nLevelExp <= MaxSkillXP; nLevelExp = ExperienceAtLevel(n++))
                if (experience > nLevelExp && ExperienceAtLevel(n + 1) > experience)
                    return (int)n;

            throw new ArgumentException("No level could be calculated from experience " + experience.ToString());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Skill))
                return false;

            Skill match = (Skill)obj;

            if ((this.SkillsName.Equals(match.SkillsName)) && (this.GoalExperience == match.GoalExperience) && (this.CurrentExperience == match.CurrentExperience))
                return true;

            return false;
        }
        
        public byte[] GetHashCode()
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] name_block = Encoding.Default.GetBytes(this.Skillname);
            byte[] expr_block = BitConverter.GetBytes(this.CurrentExperience);
            byte[] time_block = BitConverter.GetBytes(DateTime.Now.Ticks);

            md5.TransformBlock(name_block, 0, name_block.Length, name_block, 0);
            md5.TransformBlock(expr_block, 0, expr_block.Length, expr_block, 0);
            md5.TransformFinalBlock(time_block, 0, time_block.Length);

            return md5.Hash;
        }

        public override string ToString()
        {
            string thisObject = "{ [" + this.SkillsName.ToString() + "] [Current level " + CurrentLevel.ToString() +
                                " (" + this.CurrentExperience.ToString() + "xp) [" + "Goal level " + GoalLevel.ToString() +
                                " (" + this.GoalExperience.ToString() + "xp)]";
            return thisObject;
        }

        public static Skill Empty { get { return new Skill(); } }

        public static void TestHarness()
        {
            for (int i = 0; i <= 200; i++)
                System.Diagnostics.Debug.WriteLine(ExperienceAtLevel(i));
        }
    }
}
