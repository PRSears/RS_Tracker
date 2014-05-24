using System;
using System.Linq;
using System.Windows.Forms;
using RS_Goal_Tracker.Data;
using System.Collections.Generic;

namespace RS_Goal_Tracker.Interface
{
    public class UserTrackerTab:TabPage
    {
        public event ControlUpdatedEventHandler TabUpdated;
        public event TabClosingEventHandler TabClosing;
        
        protected virtual void OnControlUpdated()
        {
            ControlUpdatedEventHandler handler = TabUpdated;
            if (handler != null) handler(this);
        }
        protected virtual void OnTabClosing()
        {
            TabClosingEventHandler handler = TabClosing;
            if (handler != null) handler(this);
        }

        private Panel topPanel;
        private FlowLayoutPanel flowPanel;
        private TextBox usernameTextBox;
        private Button addSkillbarButton;
        private Button getStatsButton;
        private XButton closeTabButton;
        private ToolTip closeTabTooltip;

        private string defaultUsername = Properties.Settings.Default.DefaultUsername;
        private Skills defaultSkill = Skills.Parse(Properties.Settings.Default.DefaultSkill);

        public Player TrackingPlayer { get; set; }
        public List<SkillTrackerBar> TrackingSkills { get; set; }
        
        public override string Text
        {
            get 
            {
                if (TrackingPlayer != null)
                    return TrackingPlayer.Username;
                else
                    return defaultUsername;
            }
        }

        public UserTrackerTab():base()
        {
            TrackingPlayer = new Player(defaultUsername);
            TrackingSkills = new List<SkillTrackerBar>();

            this.InitializeComponents();
            this.Refresh();
        }

        public UserTrackerTab(Player trackingPlayer, List<SkillTrackerBar> trackingSkills):base()
        {
            TrackingPlayer = trackingPlayer;
            TrackingSkills = trackingSkills;

            foreach (SkillTrackerBar bar in TrackingSkills)
            {
                // Update this.TrackingPlayer with values from the SkillTrackerBar
                this.TrackingPlayer.UpdateSkill(bar.TrackingSkill, true, true); 

                // Subscribe to BarClosing & ControlUpdated events for existing SkillTrackerBars
                bar.BarClosing += new BarClosingEventHandler(skillTrackerBar_Closing);
                bar.BarUpdated += new ControlUpdatedEventHandler(skillTrackerBar_Updated);
                bar.DragOver += new DragEventHandler(skillTrackerBar_DragOver);
            }

            Highscores.Update(this.TrackingPlayer);

            this.InitializeComponents();
            this.Refresh();
        }

        private void skillTrackerBar_DragOver(object sender, DragEventArgs e)
        {
            if (!(sender is SkillTrackerBar))
                return;
                        
            SwapBars(
                e.Data.GetData(typeof(SkillTrackerBar)) as SkillTrackerBar,
                sender as SkillTrackerBar
                );

            OnControlUpdated();
        }

        private void SwapBars(SkillTrackerBar a, SkillTrackerBar b)
        {
            try
            {
                int a_index = TrackingSkills.IndexOf(a);
                int b_index = TrackingSkills.IndexOf(b);

                TrackingSkills[a_index] = b;
                TrackingSkills[b_index] = a;

                flowPanel.Controls.SetChildIndex(a, b_index);
            }
            catch(IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("One of the controls being swapped was not open.");
            }
        }

        public UserTrackerTab(Player trackingPlayer):this(trackingPlayer, new List<SkillTrackerBar>())
        {
        }

        private void InitializeComponents()
        {
            this.topPanel = new Panel();
            this.flowPanel = new FlowLayoutPanel();
            this.usernameTextBox = new TextBox();
            this.addSkillbarButton = new Button();
            this.getStatsButton = new Button();
            this.closeTabButton = new XButton();
            this.closeTabTooltip = new ToolTip();

            this.UseVisualStyleBackColor = true;

            //
            // topPanel
            //
            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Size = new System.Drawing.Size(0, 32);
            //
            // flowPanel
            //
            this.flowPanel.Padding = new Padding(0, 0, 0, 20);
            this.flowPanel.Dock = DockStyle.Fill;
            this.flowPanel.AutoScroll = true;
            this.AllowDrop = true;
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(1, 6);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(248, 20);
            this.usernameTextBox.TabIndex = 0;
            this.usernameTextBox.Text = (TrackingPlayer != null) ? this.TrackingPlayer.Username : defaultUsername;
            // 
            // getStatsButton
            // 
            this.getStatsButton.Location = new System.Drawing.Point(255, 6);
            this.getStatsButton.Name = "getStatsButton";
            this.getStatsButton.Size = new System.Drawing.Size(60, 20);
            this.getStatsButton.TabIndex = 1;
            this.getStatsButton.Text = "Refresh";
            this.getStatsButton.UseVisualStyleBackColor = true;
            this.getStatsButton.Click += new EventHandler(getStatsButton_Click);
            // 
            // addSkillbarButton
            // 
            this.addSkillbarButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addSkillbarButton.Location = new System.Drawing.Point(3, 32);
            this.addSkillbarButton.Name = "addSkillbarButton";
            this.addSkillbarButton.Size = new System.Drawing.Size(623, 20);
            this.addSkillbarButton.TabIndex = 2;
            this.addSkillbarButton.Text = " + Track another skill";
            this.addSkillbarButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addSkillbarButton.UseVisualStyleBackColor = true;
            this.addSkillbarButton.Click += new System.EventHandler(addSkillbarButton_Click);
            //
            // closeTabButton
            //
            this.closeTabButton.Location = new System.Drawing.Point(618, 0);
            this.closeTabButton.Click += new System.EventHandler(closeTabButton_Click);
            this.closeTabTooltip.SetToolTip(closeTabButton, "Close this user tracker tab.");

            this.Controls.Add(this.topPanel);
            this.topPanel.Controls.Add(this.usernameTextBox);
            this.topPanel.Controls.Add(this.getStatsButton);
            this.topPanel.Controls.Add(this.closeTabButton);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.addSkillbarButton);
        }
        
        private void getStatsButton_Click(object sender, EventArgs e)
        {
            if (this.usernameTextBox.Text != this.TrackingPlayer.Username)
                this.TrackingPlayer = new Player(this.usernameTextBox.Text); // User has changed the player they wish to track with this tab

            if (!Highscores.Update(this.TrackingPlayer))
                this.usernameTextBox.SelectAll();

            if (this.Parent.Parent is MainForm)
                (this.Parent.Parent as MainForm).PushStatus("Highscores updated for " + this.TrackingPlayer.Username);

            base.Text = this.Text;
            this.Refresh();
        }

        private void closeTabButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to close this tab?\nDoing so will stop tracking of this user and delete all skill progress bars.\nNote: Doing so will NOT remove anything from the database.", "Confirm close", MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
                OnTabClosing();
        }

        public override void Refresh()
        {
            System.Diagnostics.Debug.WriteLine("TabPage.Refresh() called @" + DateTime.Now.ToLongTimeString());

            foreach (SkillTrackerBar bar in this.TrackingSkills)
            {
                // If this tab's player has changed, update SkillTrackerBar to reflect changes
                if (bar.TrackingPlayer != this.TrackingPlayer)
                    bar.TrackingPlayer = this.TrackingPlayer;
                
                // Add any new SkillTrackerBars to flowPanel
                if (!this.flowPanel.Controls.Contains(bar))
                    this.flowPanel.Controls.Add(bar);

                bar.Refresh();
            }

            // Remove any old SkillTrackerBars from flowpanel
            foreach (Control control in this.flowPanel.Controls)
            {
                if (!this.TrackingSkills.Contains(control))
                {
                    this.flowPanel.Controls.Remove(control);
                    control.Dispose();
                }
            }

            OnControlUpdated();
            base.Refresh();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (msg.WParam.ToInt32() == (int)Keys.Escape || msg.WParam.ToInt32() == (int)Keys.Enter)
                    this.getStatsButton.Focus();
            }
            catch(Exception e)
            {
                Console.WriteLine("Key overide event error: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void skillTrackerBar_Closing(object sender)
        {
            SkillTrackerBar closedBar;
            if(!(sender is SkillTrackerBar)) return;
            else closedBar = (sender as SkillTrackerBar);
            
            this.TrackingSkills.Remove(closedBar);
            closedBar.Dispose();

            this.Refresh();
        }

        private void skillTrackerBar_Updated(object sender)
        {
            OnControlUpdated(); // pass it on
        }

        private void addSkillbarButton_Click(object sender, EventArgs e)
        {
            AddNewSkillBar(this.TrackingPlayer, this.defaultSkill);
        }

        /// <summary>
        /// Creates a new SkillTrackerBar object with this Tab's player and default skill, and subscribes to its BarClosing even.
        /// </summary>
        public void AddNewSkillBar(Player TrackedPlayer, Skills SkillName)
        {
            SkillTrackerBar newBar = new SkillTrackerBar(TrackingPlayer, SkillName);

            newBar.BarClosing += new BarClosingEventHandler(skillTrackerBar_Closing);
            newBar.BarUpdated += new ControlUpdatedEventHandler(skillTrackerBar_Updated);
            newBar.DragOver += new DragEventHandler(skillTrackerBar_DragOver);

            this.TrackingSkills.Add(newBar);
            this.Refresh();
        }

        public bool ContainsSkill(Skills skill)
        {
            foreach(SkillTrackerBar skillBar in this.TrackingSkills)
            {
                if (skillBar.TrackingSkill.SkillsName == skill)
                    return true;
            }

            return false;
        }
    }

    public delegate void TabClosingEventHandler(object sender);
}

//TODO make mouse wheel scroll the page