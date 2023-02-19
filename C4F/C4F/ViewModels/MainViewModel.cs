using C4F.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace C4F.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public SettingsViewModel Settings { get; set; }
        public LoggerViewModel Logger { get; set; }
        public ValidatorViewModel Validator { get; set; }
        public DownloaderViewModel Downloader { get; set; }

        public DelegateCommand DownloadCommand { get; set; }
        public DelegateCommand GithubCommand { get; set; }
        public DelegateCommand DiscordCommand { get; set; }

        public MainViewModel()
        {
            Logger = new LoggerViewModel();

            LoadModels();
            LoadCommands();
            Load();
        }

        private void Load()
        {
            Logger.Record("loading client");

            Settings.LoadVersion();

            Logger.Record("client loaded");
        }

        private void LoadModels()
        {
            Logger.Record("loading client");

            Settings = new SettingsViewModel();
            Validator = new ValidatorViewModel();
            Downloader = new DownloaderViewModel(Settings.Root, Logger);

            Logger.Record("client loaded");
        }

        private void LoadCommands()
        {
            Logger.Record("loading commands");

            DownloadCommand = new DelegateCommand(Download);
            GithubCommand = new DelegateCommand(Github);
            DiscordCommand = new DelegateCommand(Discord);

            Logger.Record("commands loaded");
        }

        private void Download(object data)
        {
            string name = $"{data}";

            if (Valid(name) == true)
            {
                Downloader.Download(name);
            }
            else
            {
                Logger.Record($"name '{name}' invalid");
            }
        }

        private void Github(object data)
        {
            System.Diagnostics.Process.Start("https://github.com/Neotoxic-off");
        }

        private void Discord(object data)
        {
            System.Diagnostics.Process.Start("https://discord.gg/vW5PA5VASb");
        }

        private bool Valid(string name)
        {
            if (Validator.Empty(name) == false)
            {
                return (name.Length > 2);
            }

            return (false);
        }
    }
}
