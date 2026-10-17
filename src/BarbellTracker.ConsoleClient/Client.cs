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
        static string filePath;
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: BarbellTracker.ConsoleClient <path-to-tracking.json>");
                return 1;
            }
            filePath = args[0];

            using IHost host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureServices((_, services) =>
                    services.AddSingleton<ServiceCache<Velocity>>()
                    .AddSingleton<ServiceCache<Acceleration>>()
                    .AddSingleton<ServiceCache<VelocityCSVModel>>()
                    .AddSingleton<ServiceCache<AccelerationCSVModel>>()
                    .AddSingleton<ServiceCache<PhysicalMotion>>()
                    .AddSingleton<ServiceCache<SetAnalysis>>()
                    .AddSingleton<ServiceCache<SetCSVModel>>()
                    .AddSingleton<AnalysisSettings>()
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
                    .AddTransient<ICalculator<PhysicalMotion>, PhysicalMotionCalculator>()
                    .AddTransient<ICalculator<SetAnalysis>, SetAnalyzer>()
                    .AddTransient<SetCSVTranslater>()
                    .AddTransient<SetMetricsToCSVFile>()
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
            var Processing5 = provider.GetRequiredService<SetMetricsToCSVFile>();
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
            return 0;
        }

        public static void handelFile(SelectFile SelectFile)
        {
            eventSystem.Fire(new FileSelected() { FilePath = filePath });
        }
    }
}
