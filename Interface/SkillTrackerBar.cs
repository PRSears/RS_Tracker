using System;
using System.Linq;
using System.Windows.Forms;
using RS_Goal_Tracker.Data;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;


namespace RS_Goal_Tracker.Interface
{
    public class SkillTrackerBar:Panel
    {
        public static Size BarSize = new Size(618, 28);
        public event ControlUpdatedEventHandler BarUpdated;
        public event BarClosingEventHandler BarClosing;
        
        protected virtual void OnBarClosing()
        {
            BarClosingEventHandler handler = BarClosing;
            if (handler != null) handler(this);
        }

        protected virtual void OnControlUpdated()
        {
            ControlUpdatedEventHandler handler = BarUpdated;
            if (handler != null) handler(this);
        }

        private bool _sortByExp;
        public bool SortByExp
        {
            get { return _sortByExp; }
            private set
            {
                _sortByExp = value;
            }
        }

        private Skills SkillName;
        public Skill TrackingSkill
        {
            get
            {
                return TrackingPlayer != null ? TrackingPlayer.GetSkill(SkillName) : null;
            }
        }

        public Player TrackingPlayer
        {
            get;
            set;
        }

        #region Components
        private ComboBox SkillsDropdown;
        private ProgressBar SkillProgressBar;
        private TextBox CurSkillExpBox;
        private TextBox GoalBox;
        private Label DividerLabel;
        private ComboBox GoalTypeDropdown;
        private XButton CloseButton;
        #endregion

        public SkillTrackerBar():base()
        {
            this.InitializeComponents();
        }

        public SkillTrackerBar(Player player, Skills skillname):base()
        {
            this.SkillName = skillname;
            this.TrackingPlayer = player;

            this.InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = SkillTrackerBar.BarSize;
            this.AllowDrop = true;
            
            this.SkillsDropdown = new ComboBox();
            this.SkillProgressBar = new ProgressBar();
            this.CurSkillExpBox = new TextBox();
            this.GoalBox = new TextBox();
            this.DividerLabel = new Label();
            this.GoalTypeDropdown = new ComboBox();
            this.CloseButton = new XButton();

            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.SkillsDropdown);
            this.Controls.Add(this.SkillProgressBar);
            this.Controls.Add(this.CurSkillExpBox);
            this.Controls.Add(this.GoalBox);
            this.Controls.Add(this.DividerLabel);
            this.Controls.Add(this.GoalTypeDropdown);

            this.CloseButton.Size = new Size(18, 14);
            this.CloseButton.Location = new Point(-3, 6);
            this.Controls.SetChildIndex(this.CloseButton, 10);
            this.CloseButton.Click += new System.EventHandler(closeButton_Click);

            this.SkillsDropdown.FormattingEnabled = true;
            foreach (Skills skill in Skills.SkillList) if(skill != Skills.Placeholder) this.SkillsDropdown.Items.Add(skill.ToString());
            this.SkillsDropdown.SelectedIndex = this.SkillsDropdown.Items.IndexOf(this.SkillName.ToString());
            this.SkillsDropdown.Location = new Point(18, this.Margin.Top);
            this.SkillsDropdown.Name = "SkillsDropdown";
            this.SkillsDropdown.Size = new Size(101, 20);
            this.SkillsDropdown.DropDownClosed += new EventHandler(SkillsDropdown_Closed);

            this.SkillProgressBar.Location = new Point(125, this.Margin.Top);
            this.SkillProgressBar.Name = "SkillProgressBar";
            this.SkillProgressBar.Size = new Size(262, 20);
            this.SkillProgressBar.MouseDown += new MouseEventHandler(SkillProgressBar_Click);

            this.CurSkillExpBox.Location = new Point(393, this.Margin.Top);
            this.CurSkillExpBox.Name = "CurSkillExpBox";
            this.CurSkillExpBox.ReadOnly = true;
            this.CurSkillExpBox.TextAlign = HorizontalAlignment.Center;
            this.CurSkillExpBox.Size = new Size(66, 20);
            this.CurSkillExpBox.Text = "0";

            this.GoalBox.Location = new Point(483, this.Margin.Top);
            this.GoalBox.Name = "GoalBox";
            this.GoalBox.TextAlign = HorizontalAlignment.Center;
            this.GoalBox.Size = new Size(66, 20);
            this.GoalBox.Text = Skill.MaxSkillXP.ToString("N0");
            this.GoalBox.TextChanged += new EventHandler(GoalBox_TextChanged);
            this.GoalBox.Enter += new EventHandler(GoalBox_Entered);
            this.GoalBox.Leave += new EventHandler(GoalBox_Left);

            this.DividerLabel.AutoSize = true;
            this.DividerLabel.Location = new Point(465, this.Margin.Top);
            this.DividerLabel.Name = "DividerLabel";
            this.DividerLabel.Size = new Size(12, 13);
            this.DividerLabel.Text = "/";

            this.GoalTypeDropdown.FormattingEnabled = true;
            this.GoalTypeDropdown.Items.AddRange(new object[] {"By Exp", "By Level"});
            this.GoalTypeDropdown.SelectedIndex = 0;
            this.GoalTypeDropdown.Location = new System.Drawing.Point(555, this.Margin.Top);
            this.GoalTypeDropdown.Name = "GoalTypeDropdown";
            this.GoalTypeDropdown.Size = new System.Drawing.Size(63, 20);
            this.GoalTypeDropdown.DropDownClosed += new EventHandler(GoalTypeDropdown_Closed);

            this.SortByExp = true;

            this.Refresh();
        }

        public override void Refresh()
        {
            // Do nothing if Tracking objects have not been initialized
            if (this.TrackingPlayer == null || this.TrackingSkill == null) return;

            //System.Diagnostics.Debug.WriteLine(" Bar.Refresh: " + TrackingSkill.CurrentExperience);

            this.CurSkillExpBox.Text = this.TrackingSkill.CurrentExperience.ToString("N0");
            this.SkillsDropdown.SelectedIndex = SkillsDropdown.Items.IndexOf(SkillName.ToString());
            if (this.SortByExp) this.GoalBox.Text = this.TrackingSkill.GoalExperience.ToString("N0");
            else this.GoalBox.Text = this.TrackingSkill.GoalLevel.ToString();

            // Update progressbar
            this.SkillProgressBar.Value = this.Progress;

            OnControlUpdated();
            base.Refresh();
        }

        public int Progress
        {
            get
            {
                double curXP = this.TrackingSkill.CurrentExperience;
                double golXP = this.TrackingSkill.GoalExperience;

                if (golXP < 1) return 0; // avoid div by 0 

                var percent = (curXP / golXP) * 100d;

                if (percent > 100) return 100;
                else if (percent < 0) return 0;
                else return (int)Math.Round(percent);
            }
        }
        
        private void SkillProgressBar_Click(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(this, DragDropEffects.Scroll | DragDropEffects.Move);
        }

        private void GoalTypeDropdown_Closed(object sender, EventArgs e)
        {
            if ((GoalTypeDropdown.SelectedItem as String).Contains("Exp"))
                this.SortByExp = true;
            else
                this.SortByExp = false;

            this.Refresh();
        }

        private void GoalBox_TextChanged(object sender, EventArgs e)
        {
            string sanitize = new string(GoalBox.Text.Where(ch => char.IsNumber(ch)).ToArray());
            long num = (sanitize != string.Empty ? long.Parse(sanitize) : 0L);
            
            // Update the skill we're tracking
            if (SortByExp)
            {
                if (num < long.MaxValue)
                    TrackingSkill.GoalExperience = Math.Abs(num);
                else
                    TrackingSkill.GoalExperience = Skill.MaxSkillXP;
            }
            else
            {
                if (num < Skill.MaxLevel)
                    TrackingSkill.GoalLevel = (int)Math.Abs(num);
                else
                    TrackingSkill.GoalLevel = Skill.MaxLevel;
            }

            //this.Refresh();
        }
        private void GoalBox_Entered(object sender, EventArgs e)
        {
            this.GoalBox.Text = SortByExp ? this.TrackingSkill.GoalExperience.ToString() : this.TrackingSkill.GoalLevel.ToString();
        }

        private void GoalBox_Left(object sender, EventArgs e)
        {
            this.GoalBox.Text = SortByExp ? this.TrackingSkill.GoalExperience.ToString("N0") : this.TrackingSkill.GoalLevel.ToString();
            this.Refresh();
        }            

        private void closeButton_Click(object sender, EventArgs e)
        {
            OnBarClosing();
        }

        private void SkillsDropdown_Closed(object sender, EventArgs e)
        {
            this.SkillName = Skills.Parse((string)this.SkillsDropdown.SelectedItem);
            this.Refresh();
        }
    }

    public delegate void BarClosingEventHandler(object sender);
    public delegate void ControlUpdatedEventHandler(object sender);
}
