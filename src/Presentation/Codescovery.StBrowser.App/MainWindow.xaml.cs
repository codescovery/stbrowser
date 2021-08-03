using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using Codescovery.StBrowser.App.Helpers;
using Codescovery.StBrowser.App.Services;

namespace Codescovery.StBrowser.App
{
    public partial class MainWindow : Window
    {
        private readonly string _defaultUrl;
        private IWpfWebBrowser _currentBrowser;
        private readonly BrowserService _browserService;

        public MainWindow(ConfigurationService configurationService, string initialUrl = null, string title = null)
        {
            InitializeComponent();
            _browserService = new BrowserService();
            _defaultUrl = string.IsNullOrWhiteSpace(initialUrl) ? configurationService.Configuration.DefaultUrl : initialUrl;
            Title = string.IsNullOrWhiteSpace(title) ? configurationService.Configuration.Title : title;
            SetupEvents();
        }

        private void SetupEvents()
        {
            PreviewKeyDown += OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                _currentBrowser.Reload(Keyboard.Modifiers == ModifierKeys.Control);
                e.Handled = true;
                return;
            }
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;
            if (e.Key == Key.T)
            {
                CreateAndAddNewTab();
                e.Handled = true;
                return;
            }
            

        }


        private void AddNewTab_OnPreviewMouseDown(object sender, RoutedEventArgs e)
        {
            CreateAndAddNewTab();
            e.Handled = true;
        }

        private void CreateAndAddNewTab()
        {
            var tabItem = new TabItem() { Header = $"New Tab", UseLayoutRounding = true };
            var tabIndex = BrowserTabs.Items.Count - 1;
            
            var browser = _browserService.CreateBrowser(_defaultUrl, tabItem, new Size(ActualWidth,ActualHeight));
            _currentBrowser = browser;
            SizeChanged +=(sender, args) =>  OnSizeChanged(sender,args,browser);
            tabItem.GotFocus +=(sender, args) => TabItemOnGotFocus(sender,args,browser);
            tabItem.PreviewMouseDown += (o, args) =>  TabItemOnPreviewMouseDown(o, args,tabItem, browser);
            var menu = CreateDockPanelBrowserMenu(browser, tabItem);
            var dockPanel = CreateTabDockPanel(menu, browser);
            tabItem.Content = dockPanel;
            BrowserTabs.Items.Insert(tabIndex, tabItem);
            BrowserTabs.SelectedIndex = tabIndex;
        }

        private void TabItemOnPreviewMouseDown(object sender, MouseButtonEventArgs args, TabItem tabItem,
            ChromiumWebBrowser browser)
        {
            if (args.MiddleButton == MouseButtonState.Pressed)
            {
                browser.Dispose();
                if (BrowserTabs.Items.Contains(tabItem))
                    BrowserTabs.Items.Remove(tabItem);
                args.Handled = true;
                return;
            }

            if (args.XButton1 == MouseButtonState.Pressed)
            {
                if (browser.CanGoBack)
                    browser.Back();
                args.Handled = true;
                return;
            }
            if (args.XButton2 == MouseButtonState.Pressed)
            {
                if (browser.CanGoForward)
                    browser.Forward();
                args.Handled = true;
                return;
            }
            args.Handled = false;
        }

        private void TabItemOnGotFocus(object sender, RoutedEventArgs e, ChromiumWebBrowser browser)
        {
            _currentBrowser = browser;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e, ChromiumWebBrowser browser)
        {
                browser.Dispatcher.Invoke(() =>
                {
                    browser.MinWidth = Math.Round(e.NewSize.Width);
                    browser.MinHeight = Math.Round(e.NewSize.Height);
                });
        }

        private DockPanel CreateTabDockPanel(Menu menu, IWpfWebBrowser browser)
        {
            var dockPanel = new DockPanel()
            {
                SnapsToDevicePixels = true,
                LastChildFill = false,
                UseLayoutRounding = true,
            };

            dockPanel.Children.Add(menu);

            dockPanel.Children.Add(browser as ChromiumWebBrowser ?? throw new InvalidOperationException());
            RenderOptions.SetBitmapScalingMode(dockPanel, BitmapScalingMode.HighQuality);

            DockPanel.SetDock(menu, Dock.Top);

            return dockPanel;
        }

       

        private Menu CreateDockPanelBrowserMenu(IWpfWebBrowser browser, TabItem tabItem)
        {
            var menu = new Menu() { MaxHeight = 150, MinWidth = ActualWidth };
            var currentUrl = new TextBox() { MinWidth = ActualHeight, Width = ActualWidth, Text = _defaultUrl };
            SizeChanged += (o, args) =>
            {
                menu.Dispatcher.Invoke(() =>
                {
                    menu.MinWidth = args.NewSize.Width;
                    menu.Width = args.NewSize.Width;
                });
                currentUrl.Dispatcher.Invoke(() =>
                {
                    currentUrl.MinWidth = args.NewSize.Width;
                    Width = args.NewSize.Width;
                });
            };
            browser.LoadingStateChanged += (sender, args) =>
                currentUrl.Dispatcher.Invoke(() =>
                {
                    return currentUrl.Text = browser.Address;
                });
            currentUrl.KeyDown += (sender, args) =>
            {
                if (args.Key != Key.Enter) return;
                try
                {
                    _browserService.NavigateToUrl(browser,  currentUrl.Text, tabItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            };
            menu.Items.Add(currentUrl);

            return menu;
        }
    }
}
