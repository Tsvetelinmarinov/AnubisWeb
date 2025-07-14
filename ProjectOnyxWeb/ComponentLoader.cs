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
        // Tool tip for theme button and home button and for the search box
        private static readonly ToolTip tip = new();

        #region Interface

        /// <summary>
        ///  Set the properties of the window like Title, Size, Location etc.
        /// </summary>
        /// <param name="window">The window wich properties will be set</param>
        internal static void SetWinProperties(Form window)
        {
            window.Text = "Onyx Web";
            window.Size = new(1800, 1000);
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
            box!.ForeColor = Color.DarkGray;
            tip.SetToolTip(box, "Enter a web adress or a site name");

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
            tip.SetToolTip(buttons[3]!, "Go home");

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

        /// <summary>
        ///  Creates a button who shows appearance window.
        /// </summary>
        /// <param name="window"></param>
        internal static void SetAppearanceOptions(Form window, TextBox searchBox, WebView2 engine)
        {
            PictureBox button = new()
            {
                Size = new(18, 18),
                Location = new(150, 8),
                Image = Properties.Resources.theme,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            button.MouseClick += (sender, eventArgs) => OpenAppearanceWindow(window, searchBox, engine);
            tip.SetToolTip(button, "Change the theme");

            window.Controls.Add(button);
        }

        #endregion

        #region System Funtionality

        /// <summary>
        ///  Opens appearance window
        /// </summary>
        private static void OpenAppearanceWindow(Form wind, TextBox searchBox, WebView2 engine)
        {
            Form appearance = new()
            {
                Size = new(300, 100),
                StartPosition = FormStartPosition.CenterParent,
                Text = "Choose theme",
                BackColor = Color.GhostWhite,
                MaximizeBox = false,
                Icon = Properties.Resources.changeTheme
            };

            PictureBox themeIcon = new()
            {
                Size = new(23, 23),
                Location = new(240, 17),
                Image = Properties.Resources.theme,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            Label themeLabel = new()
            {
                Text = "theme",
                Font = new("Cascadia Code", 11, FontStyle.Regular | FontStyle.Italic),
                ForeColor = Color.FromArgb(60, 60, 60),
                Size = new(50, 20),
                Location = new(10, 20)
            };

            ComboBox themeBox = new()
            {
                Size = new(150, 40),
                Location = new(70, 15),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.GhostWhite,
                Font = new("Cascadia Code", 11, FontStyle.Regular),
                SelectedText = "Choose option"
            };
            themeBox.Items.Add("ligth");
            themeBox.Items.Add("dark");
            themeBox.SelectedIndexChanged += (sender, eventArgs) =>
            {
                if (themeBox.SelectedIndex.Equals(0))
                {
                    SetLigthTheme(wind, appearance, searchBox, engine);
                }
                else if (themeBox.SelectedIndex.Equals(1))
                {
                    SetDarkTheme(wind, appearance, searchBox, engine);
                }
            };

            appearance.MinimumSize = appearance.Size;
            appearance.MaximumSize = appearance.Size;
            appearance.Controls.Add(themeBox);
            appearance.Controls.Add(themeIcon);
            appearance.Controls.Add(themeLabel);
            appearance.Visible = true;
        }

        /// <summary>
        ///  Sets the dark theme of the application
        /// </summary>
        private static void SetDarkTheme(Form window, Form appearanceWind, TextBox searchBox, WebView2 engine)
        {
            window.BackColor = Color.FromArgb(30, 30, 30);
            appearanceWind.BackColor = window.BackColor;
            searchBox.BackColor = Color.FromArgb(40, 40, 40);
            searchBox.ForeColor = Color.FromArgb(235, 235, 235);
            engine.BackColor = Color.FromArgb(35, 35, 35);
        }

        /// <summary>
        ///  Sets the original ligth theme of the application
        /// </summary>
        private static void SetLigthTheme(Form window, Form appearanceWind, TextBox searchBox, WebView2 engine)
        {
            window.BackColor = Color.GhostWhite;
            appearanceWind.BackColor = Color.GhostWhite;
            searchBox.BackColor = Color.GhostWhite;
            searchBox.ForeColor = Color.FromArgb(235, 235, 235);
            engine.BackColor = Color.GhostWhite;
        }

        #endregion
    }
}
