using System;
using System.Collections.Generic;
using System.Linq;
using RS_Goal_Tracker.Data;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;

namespace RS_Goal_Tracker.Interface
{
    public class TrackingState
    {
        protected string InterfaceXmlPath
        {
            get
            {
                return Properties.Settings.Default.InterfaceStateFilePath;
            }
        }

        public static string NewTabString = "+";

        public string DefaultUsername
        {
            get
            {
                return Properties.Settings.Default.DefaultUsername;
            }
        }

        public Skills DefaultSkill
        {
            get
            {
                return Skills.Parse(Properties.Settings.Default.DefaultSkill);
            }
        }

        /// <summary>
        /// List of UserTrackerTabs that should currently be open (visible).
        /// </summary>
        public List<UserTrackerTab> OpenTabs
        {
            get;
            private set;
        }

        public TrackingState()
        {
            OpenTabs = new List<UserTrackerTab>();
            Load();
        }

        /// <summary>
        /// Loads previous UI state from XML file on disc.
        /// </summary>
        public void Load()
        {
            if (!File.Exists(this.InterfaceXmlPath))
                return; // nothing to load

            XDocument interfaceXML = XDocument.Load(this.InterfaceXmlPath);

            var newTabs = new List<UserTrackerTab>(
                from tab in interfaceXML.Elements("InterfaceState").Elements("Tab")
                select new UserTrackerTab(new Player(tab.Element("Username").Value), new List<SkillTrackerBar>(
                    from skills in tab.Elements("Skill")
                    let s = new Skill(Skills.Parse(skills.Element("Skillname").Value), 
                                      long.Parse(skills.Element("CurrentExperience").Value), 
                                      int.Parse(skills.Element("Rank").Value), 
                                      long.Parse(skills.Element("GoalExperience").Value))
                    let p = new Player(tab.Element("Username").Value)
                    select new SkillTrackerBar(p.UpdateSkill(s, true, true), s.SkillsName))));

            this.OpenTabs = newTabs;
        }

        /// <summary>
        /// Saves the current UI state to an XML file on disc.
        /// </summary>
        public void Save()
        {
            var tabElements = new XElement("InterfaceState",
                from tab in OpenTabs
                select new XElement("Tab",
                    new XElement("Username", tab.TrackingPlayer.Username),
                    from skill in tab.TrackingSkills
                    let s = skill.TrackingSkill
                    select new XElement("Skill",
                        new XElement("Skillname", s.SkillsName.ToString()),
                        new XElement("CurrentExperience", s.CurrentExperience),
                        new XElement("Rank", s.Rank),
                        new XElement("GoalExperience", s.GoalExperience)) // skill node
                    ) // Tab node
                ); // root (InterfaceState) node

            XDocument interfaceXML = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment(" RS Tracker Interface Data "),
                new XComment(" Don't edit this unless you're familiar with XML "),
                new XComment(" NOTE: This file only stores what stats are currently *displayed* \n" + 
                             "     The historical data of ALL stats is stored in LocalScores.mdf "),
                tabElements);

            interfaceXML.Save(this.InterfaceXmlPath);
        }

        #region Deprecated
        [Obsolete]
        private void AltLoad()
        {
            // This (should) have the same result as this.Load(). 
            // The only difference is AltLoad uses foreach loops instead of LINQ queries.

            if (!File.Exists(this.InterfaceXmlPath))
                return; // nothing to load
            
            XDocument interfaceXML = XDocument.Load(this.InterfaceXmlPath);
            foreach (var tab in interfaceXML.Elements("InterfaceState").Elements("Tab"))
            {
                UserTrackerTab loadedTab = new UserTrackerTab();
                loadedTab.TrackingPlayer = new Player(tab.Element("Username").Value);

                foreach (var skill in tab.Elements("Skill"))
                {
                    // Build a new Skill object with data from the XElement
                    Skill loadedSkill = new Skill(
                        Skills.Parse(skill.Element("Skillname").Value),
                        long.Parse(skill.Element("CurrentExperience").Value),
                        int.Parse(skill.Element("Rank").Value),
                        long.Parse(skill.Element("GoalExperience").Value));

                    // Create a new tracker bar
                    SkillTrackerBar loadedSkillBar = new SkillTrackerBar(loadedTab.TrackingPlayer, loadedSkill.SkillsName);

                    // Update the skill attached to the tracked player
                    loadedTab.TrackingPlayer.UpdateSkill(loadedSkill, true, false);

                    // Add the SkillTrackerBar to the new (loaded)Tab
                    loadedTab.TrackingSkills.Add(loadedSkillBar);
                }

                // Add the new tab to the list of open tabs
                this.OpenTabs.Add(loadedTab);
            }
        }

        // I started implementing the save/load methods in a really stupid way.
        // It eventually clicked that I could use LINQ queries to handle transforming
        // objects into XML.
        
        // The (bad) code below uses stupid intermediary classes to go between Player and XML.
        // I also thought I'd have to make this.OpenTabs readonly, and that the intermediary 
        // classes would have to be kept up to date by obscuring the Add method of the List.
        // This all becomes unnecessary when transforming OpenTabs directly into XElements with LINQ.

        /* The stupid way...

        [Obsolete]
        private void Update()
        {
            //trackedPlayers.Clear();

            foreach(UserTrackerTab tab in this.OpenTabs)
            {
                List<SkillPackage> trackingSkills = new List<SkillPackage>();

                foreach(SkillTrackerBar skill in tab.TrackingSkills)
                {
                    trackingSkills.Add(new SkillPackage
                    {
                        Skillname = skill.TrackingSkill.SkillName.ToString(),
                        CurrentExperience = skill.TrackingSkill.CurrentExperience,
                        GoalExperience = skill.TrackingSkill.GoalExperience,
                        Rank = skill.TrackingSkill.Rank
                    });
                }

                trackedPlayers.Add(new PlayerPackage
                {
                    Username = tab.TrackingPlayer.Username,
                    TrackingSkills = trackingSkills
                });
            }
        }

        [Obsolete]
        public void AddTab(UserTrackerTab tab)
        {
            //openTabs.Add(tab);
            this.Save();
        }

        [Obsolete]
        public void AddTab(Player player, List<SkillTrackerBar> skillTrackingList)
        {
            UserTrackerTab nTab = new UserTrackerTab();
            nTab.TrackingPlayer = player;
            nTab.TrackingSkills = skillTrackingList;

            this.AddTab(nTab);
        }

        [Obsolete]
        public void CloseTab(UserTrackerTab tab)
        {
            //openTabs.Remove(tab);
            //this.Save();
        }

        [Obsolete]
        private XDocument BaseDocument()
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment(" RS Tracker Interface Data "),
                new XComment(" Don't edit this unless you're familiar with XML "),
                new XElement("InterfaceState")
            );

            return doc;
        }

        [Obsolete]
        private class PlayerPackage
        {
            public string Username { get; set; }
            public List<SkillPackage> TrackingSkills { get; set; }
        }

        [Obsolete]
        private class SkillPackage
        {
            public string Skillname { get; set; }
            public long CurrentExperience { get; set; }
            public int Rank { get; set; }
            public long GoalExperience { get; set; }
        }
         */
        #endregion

        public static void TestHarness()
        {
            #region [Obsolete] tests
            /*
            Tracking t = new Tracking();
            UserTrackerTab nTab = new UserTrackerTab();
            nTab.TrackingPlayer = new Player("patrick");
            t.AddTab(nTab);

            Debug.WriteLine("Tabs: ");
            foreach (UserTrackerTab tab in t.OpenTabs)
                Debug.WriteLine(tab.TrackingPlayer.ToString());

            Debug.WriteLine("\nAttempting to change this.OpenTabs...");
            try
            {
                t.OpenTabs.Add(new UserTrackerTab());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            Debug.WriteLine("\nAttempting to change a /tab/ in this.OpenTabs");
            try { t.OpenTabs[0].TrackingPlayer = new Player("not patrick"); }
            catch (Exception e) { Debug.WriteLine(e.Message); }

            foreach (UserTrackerTab tab in t.OpenTabs)
                Debug.WriteLine(tab.TrackingPlayer.ToString());
             */
            #endregion

            // ---- Save

            TrackingState interfaceState = new TrackingState();
            
            UserTrackerTab tab1 = new UserTrackerTab();
            tab1.TrackingPlayer = new Player("p atrick");
            tab1.TrackingPlayer.DEBUG_DummyFill();
            tab1.TrackingSkills.Add(new SkillTrackerBar(tab1.TrackingPlayer, Skills.Agility));
            tab1.TrackingSkills.Add(new SkillTrackerBar(tab1.TrackingPlayer, Skills.Cooking));

            UserTrackerTab tab2 = new UserTrackerTab();
            tab2.TrackingPlayer = new Player("p atrick");
            tab2.TrackingPlayer.DEBUG_DummyFill();
            tab2.TrackingSkills.Add(new SkillTrackerBar(tab2.TrackingPlayer, Skills.Strength));
            tab2.TrackingSkills.Add(new SkillTrackerBar(tab2.TrackingPlayer, Skills.Attack));

            interfaceState.OpenTabs.Clear();
            interfaceState.OpenTabs.Add(tab1);
            interfaceState.OpenTabs.Add(tab2);

            interfaceState.Save();

            // ---- Load

            TrackingState interfaceState2 = new TrackingState();
            foreach(UserTrackerTab tab in interfaceState2.OpenTabs)
            {
                Debug.WriteLine("(" + tab.TrackingPlayer.Username + ")");
                foreach(SkillTrackerBar skillBar in tab.TrackingSkills)
                {
                    Debug.WriteLine("    " + skillBar.TrackingSkill.SkillsName.ToString() + " " +
                        skillBar.TrackingSkill.CurrentExperience + " " +
                        skillBar.TrackingSkill.Rank + " " +
                        skillBar.TrackingSkill.GoalExperience);
                }
            }
        }
    }    
}
