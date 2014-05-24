using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RS_Goal_Tracker.Data
{
    public struct Skills
    {
        private string innerText;

        public static Skills Total { get { return new Skills("total"); } }
        public static Skills Attack { get { return new Skills("attack"); } }
        public static Skills Defence { get { return new Skills("defence"); } }
        public static Skills Strength { get { return new Skills("strength"); } }
        public static Skills Constitution { get { return new Skills("constitution"); } }
        public static Skills Ranged { get { return new Skills("ranged"); } }
        public static Skills Prayer { get { return new Skills("prayer"); } }
        public static Skills Magic { get { return new Skills("magic"); } }
        public static Skills Cooking { get { return new Skills("cooking"); } }
        public static Skills Woodcutting { get { return new Skills("woodcutting"); } }
        public static Skills Fletching { get { return new Skills("fletching"); } }
        public static Skills Fishing { get { return new Skills("fishing"); } }
        public static Skills Firemaking { get { return new Skills("firemaking"); } }
        public static Skills Crafting { get { return new Skills("crafting"); } }
        public static Skills Smithing { get { return new Skills("smithing"); } }
        public static Skills Mining { get { return new Skills("mining"); } }
        public static Skills Herblore { get { return new Skills("herblore"); } }
        public static Skills Agility { get { return new Skills("agility"); } }
        public static Skills Thieving { get { return new Skills("thieving");} }
        public static Skills Slayer { get { return new Skills("slayer"); } }
        public static Skills Farming { get { return new Skills("farming"); } }
        public static Skills Runecrafting { get { return new Skills("runecrafting"); } }
        public static Skills Hunter { get { return new Skills("hunter"); } }
        public static Skills Construction { get { return new Skills("construction"); } }
        public static Skills Summoning { get { return new Skills("summoning"); } }
        public static Skills Dungeoneering { get { return new Skills("dungeoneering");} }
        public static Skills Divination { get { return new Skills("divination"); } }
        public static Skills Placeholder { get { return new Skills("placeholder"); } }
        public static List<Skills> SkillList
        {
            get
            {
                return new List<Skills> { Skills.Total, Skills.Attack, Skills.Defence, Skills.Strength, Skills.Constitution, 
                    Skills.Ranged, Skills.Prayer, Skills.Magic, Skills.Cooking, Skills.Woodcutting, Skills.Fletching, 
                    Skills.Fishing, Skills.Firemaking, Skills.Crafting, Skills.Smithing, Skills.Mining, Skills.Herblore,
                    Skills.Agility, Skills.Thieving, Skills.Slayer, Skills.Farming, Skills.Runecrafting, Skills.Hunter, 
                    Skills.Construction, Skills.Summoning, Skills.Dungeoneering, Skills.Divination, Skills.Placeholder, 
                    Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, 
                    Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, 
                    Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, Skills.Placeholder, 
                    Skills.Placeholder, Skills.Placeholder, Skills.Placeholder };
            }
        }

        public static long ExperienceAt99 = 13034431L;

        private Skills(string InnerText) :this()
        {
            this.innerText = InnerText;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Skills))
                return false;
            Skills b = (Skills)obj;

            return this.innerText.Equals(b.innerText);
        }

        public override int GetHashCode()
        {
 	         return this.innerText.GetHashCode();
        }

        public override string ToString()
        {
            return this.innerText;
        }

        public static Skills Parse(string skillname)
        {
            // Brute force...
            foreach (Skills skill in Skills.SkillList)
            {
                if (skillname.ToLower().Equals(skill.ToString()))
                    return skill;
            }
            throw new FormatException("skillname is not the correct format.");
        }

        public static Boolean operator ==(Skills a, Skills b)
        {
            return a.Equals(b);
        }

        public static Boolean operator !=(Skills a, Skills b)
        {
            return !(a == b);
        }
    }
}
