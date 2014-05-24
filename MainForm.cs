using System.Windows.Forms;
using RS_Goal_Tracker.Interface;
using RS_Goal_Tracker.Data;
using System.IO;
using System;

namespace RS_Goal_Tracker
{
    public partial class MainForm : Form
    {
        private DraggableTabControl primaryTabs;
        public static string InitialStatus = "[Beta build]";

        public MainForm()
        {
            InstanciateComponents();
            InitializeComponent();

            this.PerformLayout();
        }

        private void InstanciateComponents()
        {
            this.primaryTabs = new DraggableTabControl();
            this.primaryTabs.SuspendLayout();
            this.primaryTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.primaryTabs.Location = new System.Drawing.Point(0, 24);
            this.primaryTabs.Size = new System.Drawing.Size(632, 230);
            this.primaryTabs.Name = "primaryTabs";
            this.primaryTabs.TabIndex = 2;

            this.Controls.Add(this.primaryTabs);
            this.primaryTabs.ResumeLayout(false);
        }

        private void trackAllSkillsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (primaryTabs.SelectedTab == primaryTabs.NewTabButton || primaryTabs.SelectedTab == null)
                return;

            UserTrackerTab tab = (UserTrackerTab)primaryTabs.SelectedTab;
            
            foreach(Skills skill in Skills.SkillList)
            {
                if (!tab.ContainsSkill(skill) && skill != Skills.Placeholder)
                    tab.AddNewSkillBar(tab.TrackingPlayer, skill);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.primaryTabs.TabStates.Save(); // Make sure it's fully up to date

            this.saveFileDialog1.FileName = Properties.Settings.Default.InterfaceStateFilePath;
            this.saveFileDialog1.Filter = "XML files (*.xml)|*.xml|All files(*.*)|*.*";

            if(this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(Properties.Settings.Default.InterfaceStateFilePath, this.saveFileDialog1.FileName, true);
                }
                catch(Exception ex)
                {
                    string error_title = "There was a problem saving the file";

                    if (ex is UnauthorizedAccessException)
                        MessageBox.Show("I don't have the required permissions to save this file.", error_title, MessageBoxButtons.OK);
                    else if (ex is DirectoryNotFoundException || ex is FileNotFoundException)
                        MessageBox.Show("The directory you're trying to save to does not exist.", error_title, MessageBoxButtons.OK);
                    else if (ex is IOException)
                        MessageBox.Show("An IO Exception has occurred.\n" + ex.Message + "\n" + ex.StackTrace, error_title, MessageBoxButtons.OK);
                    else throw;

                    return;
                }
                PushStatus("Successfully exported to " + saveFileDialog1.FileName);
            }

        }

        public void PushStatus(string status)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.AppendFormat("{0} {1} at {2}", InitialStatus, status, DateTime.Now.ToShortTimeString());

            this.toolStripStatusLabel.Text = str.ToString();
        }
    }
}
