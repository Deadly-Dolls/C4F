using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace C4F.ViewModels
{
    public class ValidatorViewModel: BaseViewModel
    {
        public bool Empty(string data)
        {
            bool[] checks =
            {
                (data == string.Empty),
                (data == null)
            };

            return (checks.Contains(true));
        }
    }
}
