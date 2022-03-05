using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.ApplicationCode.EventModel;
using BarbellTracker.DomainCode;
using BarbellTracker.Plugins.Processing;
using BarbellTracker.Plugins.Tracker;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using BarbellTracker.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.ConsoleClient
{
    public class Client
    {
        static IEventSystem eventSystem;
        public static void Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureServices((_, services) =>
                    services.AddSingleton<ServiceCache<Velocity>>()
                    .AddSingleton<ServiceCache<Acceleration>>()
                    .AddSingleton<ServiceCache<VectorCSVModel>>()
                    .AddSingleton<FileManager>()
                    .AddSingleton<EventSystem>()
                    .AddTransient<ICalculator<Velocity>, VelocityCalculator>()
                    .AddTransient<ICalculator<Acceleration>, AccelerationCalculator>()
                    .AddTransient<VelocityCSVTranslater>()
                    .AddTransient<VelocityToTable>()
                    .AddTransient<JsonLoader>()

                    )
                .Build();

            var servieces = host.Services;
            var Scope = servieces.CreateScope();
            var provider = Scope.ServiceProvider;

            var tacker = provider.GetRequiredService<JsonLoader>();
            var Processing = provider.GetRequiredService<VelocityToTable>();
            var eventSystem = provider.GetRequiredService<EventSystem>();

            StartExtractionInformation startExtractionInformation = new StartExtractionInformation()
            {
                ExtractionName = "FristExtration",
                PluginName = tacker.Name
            };

            EventDelegate<SelectFile> handelFileDelegate = handelFile;
            eventSystem.Subscribe(handelFileDelegate);

            eventSystem.Fire(new ActivatePlugin() { PluginName = Processing.Name });
            eventSystem.Fire(new StartExtractVideoInfo() { StartExtractionInformation = startExtractionInformation });

            while (true) ;
        }

        public static void handelFile(SelectFile SelectFile)
        {
            eventSystem.Fire(new FileSelected() { FilePath = @"C:\Users\schal\Desktop\MyJson.json" });
        }
    }
}
