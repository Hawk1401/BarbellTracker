using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Implementation;
using BarbellTracker.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Services.Models;

namespace BarbellTracker.Plugins.Processing
{
    public class VelocityToTable : IProcessingPlugin
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private bool Activ;
        public VelocityCalculator velocityCalculator;


        public VelocityToTable()
        {
            EventSystem.Subscribe(Event.ActivatePlugin, Activate);
            EventSystem.Subscribe(Event.DeactivatePlugin, Deactivate);
            velocityCalculator = VelocityCalculator.Instance;
            Activ = false;
        }
        public bool IsActiv()
        {
            return Activ;
        }


        public async Task ProcessData(EventContext eventContext)
        {
            var trackedInformation = eventContext.Arg as TrackedInformation;
            var Velocity = velocityCalculator.GetVelocity(trackedInformation);
            var CSV = CreateCSVLines(Velocity);
            FileManager.Instance.Write("Velocity.CSV", CSV);
        }
        public async Task Activate(EventContext eventContext)
        {
            EventSystem.Subscribe(Event.ActivatePlugin, Activate);
            Activ = true;
        }
        public async Task Deactivate(EventContext eventContext)
        {
            EventSystem.Unsubscribe(Event.ActivatePlugin, Activate);
            Activ =false;
        }


        private List<string> CreateCSVLines(Velocity velocity)
        {
            double TimeStep = 1d / velocity.FPS;
            List<string> lines = new List<string>();
            string Header = "Time;velocityAsVecotr;VelocityLength";

            lines.Add(Header);

            for (int i = 0; i < velocity.Vectors.Length; i++)
            {
                var line = $"{TimeSpan.FromSeconds(i * TimeStep).ToString(@"mm\:ss\:FF")};{velocity.Vectors[i].ToString()};{velocity.Vectors[i].length()}";
                lines.Add(line);
            }

            return lines;
        }
    }
}
