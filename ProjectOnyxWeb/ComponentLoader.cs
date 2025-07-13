/// <summary>
///  Onyx Web Browser
/// </summary>

using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOnyxWeb
{
    /// <summary>
    ///  Provides static methods to create and set the components of the browser
    /// </summary>
    internal static class ComponentLoader
    {
        /// <summary>
        ///  Set the properties of the window like Title, Size, Location etc.
        /// </summary>
        /// <param name="window">The window wich properties will be set</param>
        internal static void SetWinProperties(Form window)
        {
            window.Text = "Onyx Web";
            window.Size = new(1600, 800);
            window.StartPosition = FormStartPosition.CenterScreen;
            window.Icon = Properties.Resources.secondWeb;
            window.BackColor = Color.GhostWhite;
            window.Visible = true;
            window.MaximizeBox = true;
            window.MinimizeBox = true;
            window.Dock = DockStyle.Fill;
            window.MinimumSize = window.Size;
        }

        /// <summary>
        ///  Set the web engine that will display the web pages
        /// </summary>
        /// <param name="window">T</param>
        /// <param name="engine"></param>
#pragma warning disable IDE0060
        internal static void SetWebEngine(Form window, WebView2? engine)
        {
            engine!.Size = new(window.Width - 25, window.Height - 37);
            engine!.Location = new(4, 37);
            engine!.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;          
            window.Controls.Add(engine);
        }
#pragma warning restore IDE0060

        /// <summary>
        ///  Set the search box used to type the URL adress in
        /// </summary>
        /// <param name="window"></param>
        /// <param name="box"></param>
#pragma warning disable IDE0060
        internal static void SetSearchBox(Form window, TextBox? box)
        {
            box!.Size = new(400, 22);
            box!.Location = new(window.Width - 480, 6);
            box!.Font = new("Cascadia Code", 11, FontStyle.Regular);
            box!.BackColor = Color.GhostWhite;
            box!.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            box!.BorderStyle = BorderStyle.FixedSingle;

            window.Controls.Add(box);
        }
#pragma warning restore IDE0060 

        /// <summary>
        ///  Set the interface buttons : Back, Forward, Refresh , Home and Search
        /// </summary>
        /// <param name="window"></param>
        /// <param name="buttons"></param>
        internal static async Task SetButtons(Form window, List<PictureBox?> buttons, WebView2? engine, Uri defaultPage, TextBox? searchBox)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i] = new()
                {
                    Size = new(25, 25),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                };
            }

            buttons[0]!.Location = new(10, 6);
            buttons[1]!.Location = new(45, 6);
            buttons[2]!.Location = new(90, 6);
            buttons[3]!.Location = new(1088, 6);
            buttons[4]!.Location = new(1535, 6);
            buttons[4]!.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            buttons[0]!.Image = Properties.Resources.leftArrow;
            buttons[1]!.Image = Properties.Resources.rigthArrow;
            buttons[2]!.Image = Properties.Resources.refresh;
            buttons[3]!.Image = Properties.Resources.home;
            buttons[4]!.Image = Properties.Resources.search;

            buttons[0]!.MouseClick += (sender, eventArgs) => engine!.GoBack();
            buttons[1]!.MouseClick += (sender, eventArgs) => engine!.GoForward();
            buttons[2]!.MouseClick += (sender, eventArgs) => engine!.Refresh();

            await engine!.EnsureCoreWebView2Async();

            buttons[3]!.MouseClick += (sender, eventArgs) =>
            {
                engine!.CoreWebView2.Navigate("https://www.bing.com");              
            };

            buttons[4]!.MouseClick += (sender, eventArgs) =>
            {
                if (!searchBox!.Text.EndsWith(".com") || 
                    !searchBox!.Text.EndsWith(".net") ||
                    !searchBox!.Text.EndsWith(".bg") ||
                    !searchBox!.Text.EndsWith(".org") ||
                    !searchBox!.Text.EndsWith(".edu") ||
                    !searchBox!.Text.StartsWith("https://www.")
                )
                {
                    engine!.CoreWebView2.Navigate($"https://www.{searchBox.Text}.com");
                }
                else
                {
                    engine!.CoreWebView2.Navigate(searchBox!.Text);
                }
            };

            buttons.ForEach(window.Controls.Add);
        }
    }
}
