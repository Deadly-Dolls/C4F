using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4F.Models
{
    public class FapelloModel: BaseModel
    {
        private string _name;
        public string Name {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        private int _medias;
        public int Medias
        {
            get { return _medias; }
            set { SetProperty(ref _medias, value); }
        }
    }
}
