using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows.Media.Animation;
using System.Security.Policy;
using System.Windows.Markup;

namespace C4F.ViewModels
{
    public class DownloaderViewModel: BaseViewModel
    {
        private HttpClient Client { get; set; }
        private LoggerViewModel Logger { get; set; }
        private string Root { get; set; }
        private List<(string header, string extension)> Types { get; set; }
        private string Host { get; set; }
        private int Page { get; set; }
        private int _progress = 0;
        public int Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        private string _media = "0";
        public string Media
        {
            get { return _media; }
            set { SetProperty(ref _media, value); }
        }

        private string _totalmedias = "n e o#3212";
        public string TotalMedias
        {
            get { return _totalmedias; }
            set { SetProperty(ref _totalmedias, value); }
        }

        private string _name = "Author: neo";
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private bool _downloadbutton = true;
        public bool DownloadButton
        {
            set { SetProperty(ref _downloadbutton, value); }
            get { return _downloadbutton; }
        }

        private bool _searchbutton = true;
        public bool SearchButton
        {
            set { SetProperty(ref _searchbutton, value); }
            get { return _searchbutton; }
        }

        private string _profile = "https://avatars.githubusercontent.com/u/44700383?v=4";
        public string Profile
        {
            get { return _profile; }
            set { SetProperty(ref _profile, value); }
        }

        public DownloaderViewModel(string root, LoggerViewModel logger)
        {
            Page = 1000;
            Root = root;
            Logger = logger;
            Host = "https://fapello.com";
            Client = new HttpClient();
            Types = new List<(string header, string extension)>()
            {
                ("fapello.com", "jpg"),
                ("cdn.fapello.com", "mp4")
            };
        }

        private async Task<Models.FapelloModel> Build(string name)
        {
            Models.FapelloModel model = new Models.FapelloModel()
            {
                Name = name,
                Path = $"content/{name[0]}/{name[1]}/{name}",
                Medias = await Medias(name)
            };

            return (model);
        }

        public async Task<int> Medias(string name)
        {
            string url = $"{Host}/{name}";
            string content = await Client.GetStringAsync(url);
            string[] lines = null;
            string[] splitted = null;
            int count = 0;
            bool number = false;

            if (content != null)
            {
                lines = content.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains(url) == true && line.Contains('/') == true)
                    {
                        if (line.Split('/').Length - 1 > 4)
                        {
                            splitted = line.Split('/');
                            number = int.TryParse(splitted[4], out count);
                            if (number == true)
                            {
                                return (count);
                            }
                        }
                    }
                }
            }

            return (0);
        }

        private void Reset()
        {
            Page = 1000;
            DownloadButton = true;
            SearchButton = true;
        }

        public async void Download(string name)
        {
            DownloadButton = false;
            SearchButton = false;

            Models.FapelloModel model = await Build(name);
            Name = $"{name[0].ToString().ToUpper()}{name.Substring(1)}";
            Stream content = null;
            string path = null;
            bool updated = false;
            (string url, string extension) check = (null, null);

            Logger.Record($"downloading {model.Medias} medias");
            Setup(name);
            for (int i = 0; i < model.Medias; i++)
            {
                UpdateProgress(i, model.Medias);
                check = await Validate(model, i);
                if (check.url != null)
                {
                    UpdateMedias(i);
                    if (updated == false)
                    {
                        UpdateProfile(check.url);
                        UpdateTotalMedias(model.Medias);
                        updated = true;
                    }
                    Logger.Record($"downloading {Media}");
                    content = await Client.GetStreamAsync(check.url);
                    path = $"{Root}/{model.Name}/{i}.{check.extension}";
                    using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
                    {
                        content.CopyTo(outputFileStream);
                    }
                }
            }

            UpdateProgress(model.Medias, model.Medias);
            Logger.Record($"downloaded {model.Medias} medias");
            Reset();
        }

        private void UpdateProgress(int value, int total)
        {
            if (total > 0)
                Progress = (value * 100) / total;
        }

        private void UpdateMedias(int total)
        {
            Media = $"#{FormatNumber(total)}";
        }

        private void UpdateTotalMedias(int total)
        {
            TotalMedias = $"Medias#{FormatNumber(total)}";
        }

        private void UpdateProfile(string url)
        {
            Profile = url;
        }

        private string FormatNumber(int number)
        {
            if (number < 1000)
                return (String.Format("{0:0000}", number));
            return ($"{number}");
        }

        public async Task<bool> Search(string name)
        {
            HttpResponseMessage responseMessage = null;
            string data = null;
            bool result = false;

            SearchButton = false;
            responseMessage = await Client.GetAsync($"{Host}/{name}");
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                data = await responseMessage.Content.ReadAsStringAsync();
                result = (data.Contains("<title>Fapello</title>") == false);
            }
            SearchButton = true;

            return (result);
        }

        private async Task<(string link, string extension)> Validate(Models.FapelloModel model, int i)
        {
            string url = null;
            HttpResponseMessage responseMessage = null;

            foreach ((string header, string extension) data in Types)
            {
                if (i % 1000 == 0 && i >= Page)
                {
                    Page += 1000;
                }
                
                url = $"https://{data.header}/{model.Path}/{Page}/{model.Name}_{FormatNumber(i)}.{data.extension}";
                responseMessage = await Client.GetAsync(url);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return ((url, data.extension));
                }
            }

            return ((null, null));
        }


        private void Setup(string name)
        {
            string path = $"{Root}/{name}";

            if (Directory.Exists(Root) == false)
                Directory.CreateDirectory(Root);
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }
    }
}
