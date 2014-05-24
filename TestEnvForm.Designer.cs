namespace RS_Goal_Tracker
{
    class TestPanel : System.Windows.Forms.Panel
    {
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            System.Diagnostics.Debug.WriteLine("Painting. !!!@!@#!$#%");
            // Process.Start("notepad.exe");
        }
    }

    partial class TestEnvForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.userTabsControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.addTrackerButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.userTabsControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 797);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(674, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 100;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(64, 17);
            this.toolStripStatusLabel1.Text = "Good luck.";
            // 
            // userTabsControl
            // 
            this.userTabsControl.Controls.Add(this.tabPage1);
            this.userTabsControl.Controls.Add(this.tabPage2);
            this.userTabsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userTabsControl.HotTrack = true;
            this.userTabsControl.Location = new System.Drawing.Point(0, 0);
            this.userTabsControl.Name = "userTabsControl";
            this.userTabsControl.SelectedIndex = 0;
            this.userTabsControl.Size = new System.Drawing.Size(674, 797);
            this.userTabsControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.addTrackerButton);
            this.tabPage1.Controls.Add(this.refreshButton);
            this.tabPage1.Controls.Add(this.usernameBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(666, 771);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "User 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(527, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Test Harness";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // addTrackerButton
            // 
            this.addTrackerButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addTrackerButton.Location = new System.Drawing.Point(3, 745);
            this.addTrackerButton.Name = "addTrackerButton";
            this.addTrackerButton.Size = new System.Drawing.Size(660, 23);
            this.addTrackerButton.TabIndex = 8;
            this.addTrackerButton.Text = " + Track another skill";
            this.addTrackerButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addTrackerButton.UseVisualStyleBackColor = true;
            this.addTrackerButton.Click += new System.EventHandler(this.addTrackerButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(210, 6);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 20);
            this.refreshButton.TabIndex = 7;
            this.refreshButton.Text = "Get Stats";
            this.refreshButton.UseVisualStyleBackColor = true;
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(8, 6);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(196, 20);
            this.usernameBox.TabIndex = 6;
            this.usernameBox.Text = "Enter username...";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(629, 244);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "+";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(527, 36);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(133, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Test 2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(8, 38);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(513, 701);
            this.textBox1.TabIndex = 11;
            // 
            // TestEnvForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 819);
            this.Controls.Add(this.userTabsControl);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestEnvForm";
            this.Text = "Goal Tracker - Debug";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.userTabsControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabControl userTabsControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button addTrackerButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
    }
}

