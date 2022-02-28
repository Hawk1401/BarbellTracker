using BarbellTracker.DomainCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Services.Interface
{
    public interface ICalculator<T>
    {
        public T GetCalculatedValue(TrackedInformation trackedInformation);
    }
}
