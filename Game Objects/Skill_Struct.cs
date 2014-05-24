using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_Goal_Tracker.Game_Objects
{
    public struct Skill
    {
        private string innerText;

        public static Skill Total { get { return new Skill("total"); } }
        public static Skill Attack { get { return new Skill("attack"); } }
        public static Skill Defence { get { return new Skill("defence"); } }
        public static Skill Strength { get { return new Skill("strength"); } }
        public static Skill Constitution { get { return new Skill("constitution"); } }
        public static Skill Ranged { get { return new Skill("ranged"); } }
        public static Skill Prayer { get { return new Skill("prayer"); } }
        public static Skill Magic { get { return new Skill("magic"); } }
        public static Skill Cooking { get { return new Skill("cooking"); } }
        public static Skill Woodcutting { get { return new Skill("woodcutting"); } }
        public static Skill Fletching { get { return new Skill("fletching"); } }
        public static Skill Fishing { get { return new Skill("fishing"); } }
        public static Skill Firemaking { get { return new Skill("firemaking"); } }
        public static Skill Crafting { get { return new Skill("crafting"); } }
        public static Skill Smithing { get { return new Skill("smithing"); } }
        public static Skill Mining { get { return new Skill("mining"); } }
        public static Skill Herblore { get { return new Skill("herblore"); } }
        public static Skill Agility { get { return new Skill("agility"); } }
        public static Skill Thieving { get { return new Skill("thieving");} }
        public static Skill Slayer { get { return new Skill("slayer"); } }
        public static Skill Farming { get { return new Skill("farming"); } }
        public static Skill Runecrafting { get { return new Skill("runecrafting"); } }
        public static Skill Hunter { get { return new Skill("hunter"); } }
        public static Skill Construction { get { return new Skill("construction"); } }
        public static Skill Summoning { get { return new Skill("summoning"); } }
        public static Skill Dungeoneering { get { return new Skill("dungeoneering");} }
        public static Skill Divination { get { return new Skill("divination"); } }
        public static Skill Placeholder { get { return new Skill("placeholder"); } }
        public static List<Skill> SkillList
        {
            get
            {
                return new List<Skill> { Skill.Total, Skill.Attack, Skill.Defence, Skill.Strength, Skill.Constitution, 
                    Skill.Ranged, Skill.Prayer, Skill.Magic, Skill.Cooking, Skill.Woodcutting, Skill.Fletching, 
                    Skill.Fishing, Skill.Firemaking, Skill.Crafting, Skill.Smithing, Skill.Mining, Skill.Herblore,
                    Skill.Agility, Skill.Thieving, Skill.Slayer, Skill.Farming, Skill.Runecrafting, Skill.Hunter, 
                    Skill.Construction, Skill.Summoning, Skill.Dungeoneering, Skill.Divination, Skill.Placeholder, 
                    Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, 
                    Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, 
                    Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, Skill.Placeholder, 
                    Skill.Placeholder, Skill.Placeholder, Skill.Placeholder };
            }
        }

        private Skill(string InnerText)
        {
            this.innerText = InnerText;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Skill))
                return false;
            Skill b = (Skill)obj;

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

        public static Skill Parse(string skillname)
        {
            foreach (Skill skill in Skill.SkillList)
            {
                if (skillname.Equals(skill.ToString()))
                    return skill;
            }
            throw new FormatException("skillname is not the correct format.");
        }

        public static Boolean operator ==(Skill a, Skill b)
        {
            return a.Equals(b);
        }

        public static Boolean operator !=(Skill a, Skill b)
        {
            return !(a == b);
        }
    }
}
