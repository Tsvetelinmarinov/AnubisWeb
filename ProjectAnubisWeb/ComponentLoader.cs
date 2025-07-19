/// <summary>
///  Anubis Web Browser
/// </summary>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Web.WebView2.WinForms;

namespace ProjectAnubisWeb
{
    /// <summary>
    ///  Provides static methods to create and set the components of the browser
    /// </summary>
    internal class ComponentLoader
    {
        // Tool tip for theme and home buttons and for the search box
        private static readonly ToolTip tip = new();

        //History list - keeps all the history
        private static readonly List<string>? history = [];

        #region Interface

        /// <summary>
        ///  Set the properties of the window like Title, Size, Location etc.
        /// </summary>
        /// <param name="window">The window wich properties will be set</param>
        internal static void SetWinProperties(Form window)
        {
            window.Text = "Anubis Web";
            window.Size = new(1800, 1000);
            window.StartPosition = FormStartPosition.CenterScreen;
            window.Icon = Properties.Resources.thirdIcon;
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
            engine!.NavigationCompleted += (sender, eventArgs) => AddLinkToHistory(engine!);
            window.Controls.Add(engine);           
        }
#pragma warning restore IDE0060

        /// <summary>
        ///  Set the search box used to type the URL adress in
        /// </summary>
        /// <param name="window"></param>
        /// <param name="box"></param>
#pragma warning disable IDE0060
        internal static void SetSearchBox(Form window, TextBox? box, WebView2 engine)
        {
            box!.Size = new(400, 22);
            box!.Location = new(window.Width - 480, 6);
            box!.Font = new("Cascadia Code", 10, FontStyle.Regular);
            box!.BackColor = Color.GhostWhite;
            box!.ForeColor = Color.FromArgb(80, 80, 80);
            box!.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            box!.BorderStyle = BorderStyle.FixedSingle;
            tip.SetToolTip(box, "Enter a web adress or a site name");
            box!.KeyDown += (sender, eventArgs) =>
            {
                if (eventArgs.KeyCode == Keys.Enter)
                {
                    SearchInWeb(engine, box);
                }
            };

            window.Controls.Add(box);
        }
#pragma warning restore IDE0060 

        /// <summary>
        ///  Set the interface buttons : Back, Forward, Refresh , Home and Search
        /// </summary>
        /// <param name="window"></param>
        /// <param name="buttons"></param>
        internal static async Task SetBaseButtons(Form window, List<PictureBox?> buttons, WebView2? engine, TextBox? searchBox)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i] = new()
                {
                    Size = new(22, 22),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                };
            }

            await engine!.EnsureCoreWebView2Async();

            buttons[0]!.Location = new(10, 6);
            buttons[0]!.Image = Properties.Resources.leftArrow;
            buttons[0]!.MouseClick += (sender, eventArgs) => NavigateBackward(engine!); 
            tip.SetToolTip(buttons[0]!, "Previus page");

            buttons[1]!.Location = new(45, 6);
            buttons[1]!.Image = Properties.Resources.rigthArrow;
            buttons[1]!.MouseClick += (sender, eventArgs) => NavigateForward(engine!);
            tip.SetToolTip(buttons[1]!, "Next page");

            buttons[2]!.Location = new(90, 6);
            buttons[2]!.Image = Properties.Resources.refresh;
            buttons[2]!.MouseClick += (sender, eventArgs) => ReloadPage(engine!);
            tip.SetToolTip(buttons[2]!, "Refresh page");

            buttons[3]!.Location = new(1286, 8);
            buttons[3]!.Image = Properties.Resources.home;
            buttons[3]!.MouseClick += (sender, eventArgs) => NavigateHome(engine);              
            tip.SetToolTip(buttons[3]!, "Go home");

            buttons[4]!.Location = new(1727, 7);
            buttons[4]!.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            buttons[4]!.Image = Properties.Resources.search;
            buttons[4]!.MouseClick += (sender, eventArgs) => SearchInWeb(engine, searchBox!);
            tip.SetToolTip(buttons[4]!, "Search in the web");

            buttons.ForEach(window.Controls.Add);
        }

        /// <summary>
        ///  Creates a button who shows appearance window.
        /// </summary>
        /// <param name="window"></param>
        internal static void SetAppearanceButton(Form window, TextBox searchBox, WebView2 engine)
        {
            PictureBox button = new()
            {
                Size = new(21, 21),
                Location = new(150, 7),
                Image = Properties.Resources.themeNew,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            button.MouseClick += (sender, eventArgs) => OpenAppearanceWindow(window, searchBox, engine);
            tip.SetToolTip(button, "Change the theme");

            window.Controls.Add(button);
        }

        /// <summary>
        ///  Create a button who opens window with the browsing history
        /// </summary>
        /// <param name="window"></param>
        internal static void SetHistoryButton(Form window, WebView2 engine)
        {
            PictureBox historyButton = new()
            {
                Size = new(21, 21),
                Location = new(185, 7),
                Image = Properties.Resources.historyIcon,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            historyButton.Click += (sender, eventArgs) => OpenHistoryWindow(engine);

            tip.SetToolTip(historyButton, "Show history");

            window.Controls.Add(historyButton);
        }

        /// <summary>
        ///  Creates a panel for new tabs
        /// </summary>
        /// <param name="window"></param>
        internal static void SetTabPanel(Form window)
        {
            TabControl tabs;
            PictureBox newTab;

            tabs = new()
            {
                Location = new(220, 4),
                Size = new(1000, 27),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            newTab = new()
            {
                Size = new(21, 21),
                Location = new(1225, 7),
                Image = Properties.Resources.newTab,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            newTab.Click += (sender, eventArgs) => CreateNewTab(tabs);

            tip.SetToolTip(newTab, "Open new tab");

            window.Controls.Add(newTab);
            window.Controls.Add(tabs);
        }

        #endregion

        #region System Functionality

        /// <summary>
        ///  Navigate to the next page in the browser history
        /// </summary>
        /// <param name="engine"></param>
        private static void NavigateForward(WebView2 engine)
            => engine.GoForward();

        /// <summary>
        ///  Navigate to the previus page in the browser history
        /// </summary>
        /// <param name="engine"></param>
        private static void NavigateBackward(WebView2 engine)
            => engine.GoBack();

        /// <summary>
        ///  Refresh the page
        /// </summary>
        /// <param name="engine"></param>
        private static void ReloadPage(WebView2 engine)
            => engine.CoreWebView2.Reload();

        /// <summary>
        ///  Navigate to the home page
        /// </summary>
        /// <param name="engine"></param>
        private static void NavigateHome(WebView2 engine)
            => engine.CoreWebView2.Navigate(BrowserGUI.DefaultPage.OriginalString);

        /// <summary>
        ///  Search the text in the text box in the web
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="searchBox"></param>
        private static void SearchInWeb(WebView2 engine, TextBox searchBox)
        {
            string search = searchBox.Text.Trim();

            if (Uri.IsWellFormedUriString(search, UriKind.Absolute))
            {
                engine.CoreWebView2.Navigate(search);
                engine.SourceChanged += (sender, eventArgs) => searchBox.Text = engine.CoreWebView2.Source;
            }
            else if (search.Contains('.') && !search.Contains(' '))
            {
                engine.CoreWebView2.Navigate($"https://{search}");
                engine.SourceChanged += (sender, eventArgs) => searchBox.Text = engine.CoreWebView2.Source;
            }
            else
            {
                string querry = Uri.EscapeDataString(search);
                engine.CoreWebView2.Navigate($"https://www.google.com/search?q={querry}");
                engine.SourceChanged += (sender, eventArgs) => searchBox.Text = engine.CoreWebView2.Source;
            }
        }

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
        ///  Opens window with the browsed history
        /// </summary>
        /// <param name="window"></param>
        private static void OpenHistoryWindow(WebView2 engine)
        {
            Form historyWind;
            ListBox links;
            Label info;

            historyWind = new()
            {
                Size = new(500, 800),
                StartPosition = FormStartPosition.CenterParent,
                Text = "History",
                Icon = Properties.Resources.historyWindowIcon,
                BackColor = Color.GhostWhite,
                Visible = true,
                MaximizeBox = false
            };

            links = new()
            {
                Location = new(0, 30),
                Size = new(historyWind.Width, historyWind.Height),
                ScrollAlwaysVisible = true,
                BackColor = Color.GhostWhite,
                Font = new Font("Cascadia Code", 9),
                HorizontalScrollbar = true,               
            };
            links.SelectedIndexChanged += (sender, eventArgs) => NavigateToStoredLink(engine, links);

            foreach (string link in history!)
            {
                links.Items.Add(link);
            }

            info = new()
            {
                Size = new(200, 20),
                Location = new(10, 12),
                Text = "Last visited",
                Font = new Font("Cascadia Code", 8),
            };

            historyWind.MaximumSize = historyWind.Size;
            historyWind.MinimumSize = historyWind.Size;
            historyWind.Controls.Add(links);
            historyWind.Controls.Add(info);
        } 

        /// <summary>
        ///  Adds current link to the history list
        /// </summary>
        /// <param name="engine"></param>
        private static void AddLinkToHistory(WebView2 engine)
            => history!.Add(engine.CoreWebView2.Source);

        /// <summary>
        ///  Calls when users click some of the links in the history
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="links"></param>
        private static void NavigateToStoredLink(WebView2 engine, ListBox links)
        {
            if (links.SelectedItem != null)
            {
                engine.CoreWebView2.Navigate(links.SelectedItem!.ToString());
            }
        }       

        /// <summary>
        ///  Sets the dark theme of the application
        /// </summary>
        private static void SetDarkTheme(Form window, Form? appearanceWind = null, TextBox? searchBox = null, WebView2? engine = null)
        {
            window.BackColor = Color.FromArgb(30, 30, 30);

            if (appearanceWind != null && searchBox != null && engine != null)
            {
                appearanceWind.BackColor = window.BackColor;
                searchBox.BackColor = Color.FromArgb(40, 40, 40);
                searchBox.ForeColor = Color.FromArgb(235, 235, 235);
                engine.BackColor = Color.FromArgb(35, 35, 35);
            }          
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

        /// <summary>
        ///  Calls when the New tab button (+) is pressed
        /// </summary>
        private static void CreateNewTab(TabControl tabControl)
        {
            TabPage newTab = new("New tab");
            WebView2 engine = new();

            engine.EnsureCoreWebView2Async();

            newTab.Controls.Add(engine);

            tabControl.SelectedTab = newTab;
            tabControl.TabPages.Add(newTab);
            engine.Source = BrowserGUI.DefaultPage;
        }

        #endregion
    }
}
