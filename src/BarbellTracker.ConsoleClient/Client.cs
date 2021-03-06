using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter;
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
                    .AddSingleton<ServiceCache<VelocityCSVModel>>()
                    .AddSingleton<ServiceCache<AccelerationCSVModel>>()
                    .AddSingleton<FileManager>()
                    .AddSingleton<IEventSystem, EventSystem>()
                    .AddSingleton<PluginManager>()
                    .AddSingleton<UIAdapterManager>()
                    .AddTransient<ICalculator<Velocity>, VelocityCalculator>()
                    .AddTransient<ICalculator<Acceleration>, AccelerationCalculator>()
                    .AddTransient<VelocityCSVTranslater>()
                    .AddTransient<VelocityToCSVFile>()
                    .AddTransient<VelocityToAdapterTable>()
                    .AddTransient<AccelerationCSVTranslater>()
                    .AddTransient<AccelerationToCSVFile>()
                    .AddTransient<AccelerationToAdapterTable>()
                    .AddTransient<JsonLoader>()
                    )
                .Build();

            var servieces = host.Services;
            var Scope = servieces.CreateScope();
            var provider = Scope.ServiceProvider;

            var tacker = provider.GetRequiredService<JsonLoader>();
            var Processing1 = provider.GetRequiredService<VelocityToCSVFile>();
            var Processing2 = provider.GetRequiredService<AccelerationToCSVFile>();
            var Processing3 = provider.GetRequiredService<VelocityToAdapterTable>();
            var Processing4 = provider.GetRequiredService<AccelerationToAdapterTable>();
            eventSystem = provider.GetRequiredService<IEventSystem>();
            var manager = provider.GetRequiredService<PluginManager>();
            var Adaptermanager = provider.GetRequiredService<UIAdapterManager>();


            StartExtractionInformation startExtractionInformation = new StartExtractionInformation()
            {
                ExtractionName = "FristExtration",
                PluginName = tacker.Name
            };

            EventDelegate<SelectFile> handelFileDelegate = handelFile;
            eventSystem.Subscribe(handelFileDelegate);

            manager.TurnAllProcessingPluginOn();

            eventSystem.Fire(new StartExtractVideoInfo() { StartExtractionInformation = startExtractionInformation });

            Console.WriteLine("done");
            while (true) ;
        }

        public static void handelFile(SelectFile SelectFile)
        {
            eventSystem.Fire(new FileSelected() { FilePath = @"C:\Users\schal\Desktop\MyJson.json" });
        }
    }
}
