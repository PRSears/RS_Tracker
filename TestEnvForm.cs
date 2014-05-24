using System;
using System.Windows.Forms;
using RS_Goal_Tracker.Data;
using System.Collections.Generic;

namespace RS_Goal_Tracker
{
    public partial class TestEnvForm : Form
    {
        public TestEnvForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button removed.");
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button removed.");
        }

        private void addTrackerButton_Click(object sender, EventArgs e)
        {

        }

        private void closeTabButton_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            Player p = new Player("p atrick");
            Data.DatabaseInterface db = new Data.DatabaseInterface();

            p = db.GetPlayer(p.UserID);

            StringBuilder s = new StringBuilder();
            foreach (Skill skill in p.Skills)
                s.AppendLine(skill.ToString());

            this.textBox1.Text += s;
             */


            DatabaseInterface db = new DatabaseInterface();
            
            // fill 
            Player p1 = new Player("patrick");
            Player p2 = new Player("not patrick");
            p1.DEBUG_DummyFill();
            p2.DEBUG_DummyFill();

            db.AddPlayer(p1);
            db.AddPlayer(p2);

            // /fill
                        
            Player testP1 = db.GetPlayer(p1.UserID);
            Player testP2 = db.GetPlayer(p2.UserID);

            Console.WriteLine("\n" + p1.Username + " / " + db.Usernames[1]);
            List<Skill> read = db.GetPlayer(db.UserIDs[1]).Skills;
            for(int i = 0; i < read.Count; i++)
            {
                bool pass = p1.Skills[i].Equals(read[i]);
                Console.WriteLine(read[i].Skillname + ": " + pass);
            }

            Console.WriteLine("\n" + p2.Username + " / " + db.Usernames[0]);
            read = db.GetPlayer(db.UserIDs[0]).Skills;
            for (int i = 0; i < read.Count; i++)
            {
                bool pass = p2.Skills[i].Equals(read[i]);
                Console.WriteLine(read[i].Skillname + ": " + pass);
            }

            //foreach (Player player in db.Users) Debug.WriteLine(player.ToString());

            //Debug.WriteLine(DatabaseInterface.TestDBDump());
        }
    }
}
