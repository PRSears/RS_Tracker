using System;
using System.Windows.Forms;

namespace RS_Goal_Tracker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            //Application.Run(new TestEnvForm());
        }
    }
}

// TODO attach actions to menu items
// TODO find a way to create a scheduled task
// TODO fix skilltrackerbar goalbox width to fit >1B xp
// TODO add toggle for percentage labels to skilltrackerbar and hook it to the menu button
// 