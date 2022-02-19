using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;

namespace BarbellTracker.Services.Interface
{
    public interface IService
    {
        public string Name { get; }
        public string Description { get; }
    }
}
