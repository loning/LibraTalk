using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraTalk.Windows.Client.Services
{
    public class CommandDescription
    {
        public string Name
        {
            get;
            set;
        }

        public Func<string, object, Task> Action
        {
            get;
            set;
        }
    }

}