using System;
using CefSharp;
using CefSharp.Wpf;
using Codescovery.StBrowser.App.Constants;

namespace Codescovery.StBrowser.App.Helpers
{ 
    internal  class CefSharpHelper
    {
        public static void InitilizeCef()
        {
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name ?? DefaultValues.ApplicationName;
            var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var stBrowserLocalApplicationDataPath = System.IO.Path.Combine(localApplicationData, assemblyName);
            var settings = new CefSettings
            {
                PersistUserPreferences = true,
                WindowlessRenderingEnabled = true,
                CachePath = stBrowserLocalApplicationDataPath,
                RootCachePath = stBrowserLocalApplicationDataPath,
                UserDataPath = stBrowserLocalApplicationDataPath,
                LogFile = System.IO.Path.Combine(stBrowserLocalApplicationDataPath, $"{DateTime.UtcNow} - {DefaultValues.LogSuffix}")
            };
            Cef.Initialize(settings);
        }
    }
}
