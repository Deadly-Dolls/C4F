using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4F.ViewModels
{
    public class LoggerViewModel: BaseViewModel
    {
        private string _log = "";
        public string Log
        {
            get { return (_log); }
            set { SetProperty(ref _log, value); }
        }

        public void Record(string message)
        {
            Log = message;
        }
    }
}
