using BarbellTracker.DomainCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Services.Interface
{
    public interface ICSVTranslater<T>
    {
        public T GetCSV(TrackedInformation trackedInfos);

    }
}
