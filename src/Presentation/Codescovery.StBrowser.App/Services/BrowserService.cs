using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CefSharp;
using CefSharp.DevTools.SystemInfo;
using CefSharp.Wpf;
using Codescovery.StBrowser.App.Helpers;

namespace Codescovery.StBrowser.App.Services
{
    internal class BrowserService
    {
        public BrowserService()
        {
            CefSharpHelper.InitilizeCef();
        }
        public ChromiumWebBrowser CreateBrowser(string url, TabItem tabItem, System.Windows.Size size)
        {

            var browser = new ChromiumWebBrowser(url)
            {
                MinHeight = Math.Round(size.Height),
                MinWidth = Math.Round(size.Width),
                UseLayoutRounding = true,
                BrowserSettings = new BrowserSettings(true)
                {
                    LocalStorage = CefState.Enabled,
                    ApplicationCache = CefState.Enabled,
                    ImageLoading = CefState.Enabled,
                    FileAccessFromFileUrls = CefState.Enabled,
                    Databases = CefState.Enabled
                }
            };
            browser.PreviewMouseWheel += (sender, args) =>
            {
                if (Keyboard.Modifiers != ModifierKeys.Control)
                    return;
                if (args.Delta > 0)
                    browser.Dispatcher.Invoke(() => { browser.ZoomLevel += 0.10; });

                else if (args.Delta < 0)
                    browser.Dispatcher.Invoke(() => { browser.ZoomLevel -= 0.10; });
            };
            RenderOptions.SetBitmapScalingMode(browser, BitmapScalingMode.HighQuality);

            browser.IsBrowserInitializedChanged += (sender, args)
                =>
            {
                browser.Dispatcher.Invoke(() =>
                {
                    if (!browser.IsDisposed)
                        browser.SetZoomLevel(0.8);
                });
                NavigateToUrl(browser, url, tabItem);
            };
            
            return browser;
        }
        public void NavigateToUrl(IWpfWebBrowser browser, string url, TabItem tabItem = null)
        {
            var persistedUrl = url.StartsWith("http://") || url.StartsWith("https://") ? url : $"http://{url}";
            if (browser.IsBrowserInitialized)
                browser.Load(persistedUrl);
            browser.LoadingStateChanged += (sender, args) =>
            {
                if (!args.IsLoading)
                {
                    tabItem?.Dispatcher.Invoke(() =>
                    {
                        tabItem.Header = browser.Title;
                    });

                }
            };
        }
    }
}
