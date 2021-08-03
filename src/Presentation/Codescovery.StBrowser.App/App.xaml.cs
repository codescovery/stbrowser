using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Codescovery.StBrowser.App.Models;
using Codescovery.StBrowser.App.Services;
using CommandLine;

namespace Codescovery.StBrowser.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Parser.Default.ParseArguments<CommandLineArguments>(e.Args)
                .WithParsed<CommandLineArguments>(o =>
                {
                    var mainWindow = new MainWindow(new ConfigurationService(), o.StartUrl, o.Tiltle);
                    mainWindow.Show();
                });
        }
    }
}
