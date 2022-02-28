using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.EventModel;
using BarbellTracker.DomainCode;
using BarbellTracker.Plugins.Processing;
using BarbellTracker.Plugins.Tracker;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using BarbellTracker.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BarbellTracker.ConsoleClient
{
    public class Client
    {
        public static void Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureServices((_, services) =>
                    services.AddSingleton<ServiceCache<Velocity>>()
                    .AddSingleton<ServiceCache<Acceleration>>()
                    .AddSingleton<ServiceCache<VectorCSVModel>>()
                    .AddSingleton<FileManager>()
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

            StartExtractionInformation startExtractionInformation = new StartExtractionInformation()
            {
                ExtractionName = "FristExtration",
                PluginName = tacker.Name
            };

            EventSystem.Subscribe(Event.SelectFile, handelFile);

            EventSystem.Fire(new object(), Event.ActivatePlugin, new PluginName() { Value = Processing.Name });
            EventSystem.Fire(new object(), Event.StartExtractVideoInfo, startExtractionInformation);

            while (true) ;
        }

        public static async Task handelFile(EventContext eventContext)
        {
            EventSystem.Fire(new object(), Event.FileSelected, new FilePath() { Value = @"C:\Users\schal\Desktop\MyJson.json" } );

        }
    }
}
