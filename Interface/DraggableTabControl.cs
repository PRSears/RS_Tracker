using System;
using System.Windows.Forms;

namespace RS_Goal_Tracker.Interface
{
    class DraggableTabControl : TabControl
    {
        private TabPage dragging;
        public TrackingState TabStates
        {
            get;
            set;
        }
        public TabPage NewTabButton
        {
            get;
            private set;
        }

        public DraggableTabControl()
        {
            NewTabButton = new TabPage();
            NewTabButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            NewTabButton.Name = TrackingState.NewTabString;
            NewTabButton.Text = TrackingState.NewTabString;
            NewTabButton.Size = new System.Drawing.Size(624, 204);
            NewTabButton.Padding = new System.Windows.Forms.Padding(3);

            TabStates = new TrackingState();
            foreach (UserTrackerTab tab in TabStates.OpenTabs)
                InsertExistingTab(tab);

            MouseDown += DraggableTabControl_MouseDown;
            MouseMove += DraggableTabControl_MouseMove;
            Selecting += DraggableTabControl_Selecting;
        }

        public void DraggableTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(this.SelectedTab != null)
            {
                if(this.SelectedTab.Equals(NewTabButton))
                {
                    OpenNewTab();
                }
            }

            TabStates.Save();
        }

        /// <summary>
        /// Creates a new UserTrackerTab object, adds it to OpenTabs and focuses to it.
        /// Subscribes to its TabClosing and ControlUpdated events.
        /// </summary>
        public void OpenNewTab()
        {
            UserTrackerTab newTab = new UserTrackerTab();

            InsertExistingTab(newTab);

            this.SelectTab(newTab);
        }

        public void InsertExistingTab(UserTrackerTab tab)
        {
            tab.TabClosing += new Interface.TabClosingEventHandler(tabPage_TabClosing);
            tab.TabUpdated += new Interface.ControlUpdatedEventHandler(control_Updated);

            if(!TabStates.OpenTabs.Contains(tab))
                TabStates.OpenTabs.Add(tab);
            
            SyncTabs();
        }

        public void CloseTab(TabPage tab)
        {
            TabStates.OpenTabs.Remove(tab as UserTrackerTab);

            if (TabStates.OpenTabs.Count < 1) // Open a new tab if we just removed the last one
                OpenNewTab();

            SelectNextControl(tab, false, false, true, true); // Selects the control BEFORE the tab we just closed
            SyncTabs();

            tab.Dispose();
        }

        public void SyncTabs()
        {
            this.Controls.Clear();

            foreach (TabPage tab in TabStates.OpenTabs)
                this.Controls.Add(tab);

            this.Controls.Add(NewTabButton);
        }

        public void SwapTabs(TabPage a, TabPage b)
        {
            if(!((a is UserTrackerTab) && (b is UserTrackerTab))) return;

            try
            {
                int a_index = TabStates.OpenTabs.IndexOf((UserTrackerTab)a);
                int b_index = TabStates.OpenTabs.IndexOf((UserTrackerTab)b);

                TabStates.OpenTabs[a_index] = (UserTrackerTab)b;
                TabStates.OpenTabs[b_index] = (UserTrackerTab)a;

                SyncTabs();

                SelectTab(b_index);
            }
            catch(IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("One of the tabs being swapped was not already open.");
            }
        }

        private void control_Updated(object sender)
        {
            TabStates.Save();
            Console.WriteLine("A control has been updated, and changes saved @ " + DateTime.Now.ToLongTimeString());
        }

        private void tabPage_TabClosing(object sender)
        {
            if (!(sender is TabPage)) return;

            CloseTab(sender as TabPage);
        }

        private void DraggableTabControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging == null || e.Button != System.Windows.Forms.MouseButtons.Left) return;

            TabPage hovering = TabAt(e.Location);

            if (hovering == null || hovering == dragging) return;

            SwapTabs(dragging, hovering);
        }

        private void DraggableTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            dragging = TabAt(e.Location);
        }

        private TabPage TabAt(System.Drawing.Point position)
        {
            foreach (TabPage tab in this.TabPages)
                if (this.GetTabRect(TabPages.IndexOf(tab)).Contains(position))
                    return tab;

            return null;
        }

    }
}