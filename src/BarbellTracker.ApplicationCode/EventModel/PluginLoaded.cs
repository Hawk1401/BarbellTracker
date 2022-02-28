using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.ApplicationCode.EventModel
{

    public class Name
    {
        public string Value { get; set; }
    }
    public class PluginName : Name { }
    public class AdapterPath : Name { }
    public class FilePath : Name { }


}
