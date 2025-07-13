/// <summary>
///  Onyx Web Browser
/// </summary> 

using System;
using System.Windows.Forms;

namespace ProjectOnyxWeb
{
    /// <summary>
    ///  The entry point of the application
    /// </summary>
    internal class Launcher
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new BrowserGUI());
        }
    }
}
