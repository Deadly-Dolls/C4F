using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace C4F.ViewModels
{
    public class SettingsViewModel: BaseViewModel
    {
        private string _version;
        public string Version
        {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }

        private string _root = "Downloads";
        public string Root
        {
            get { return _root; }
            set { SetProperty(ref _root, value); }
        }

        public void LoadVersion()
        {
            Version = $"{Assembly.GetExecutingAssembly().GetName().Version}";
        }
    }
}
