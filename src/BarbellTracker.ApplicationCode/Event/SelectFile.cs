using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.ApplicationCode.Event
{
    public class SelectFile
    {
        private string[] _FileExtensionRestriction { get; set; }
        // ?? sinnvoll?? TODO

        public string[] FileExtensionRestriction { 
            get 
            { 
                return _FileExtensionRestriction; 
            }

            init { 
                _FileExtensionRestriction = value; 
            } 
        }
    }
}
