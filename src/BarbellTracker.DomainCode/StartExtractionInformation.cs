using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.DomainCode
{
    public class StartExtractionInformation
    {
        public string PluginName { get; set; }
        public string Id => $"{ExtractionName}-{dateTime}";
        public string ExtractionName { get; set; }

        private string dateTime = DateTime.Now.ToString("yyyy.MM.dd--HH-mm-ss"); // maby we should use some ISO standard for the format

    }
}
