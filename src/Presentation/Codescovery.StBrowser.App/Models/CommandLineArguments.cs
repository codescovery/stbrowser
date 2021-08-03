using CommandLine;

namespace Codescovery.StBrowser.App.Models
{
    public class CommandLineArguments
    {
        [Option('u', "url", Required = false, HelpText = "Startup url.")]
        public string StartUrl { get; set; }
        [Option('t', "title", Required = false, HelpText = "Window Title.")]
        public string Tiltle { get; set; }
    }
}
