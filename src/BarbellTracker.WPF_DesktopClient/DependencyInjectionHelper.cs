using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.Plugins.Processing;
using BarbellTracker.Plugins.Tracker;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using BarbellTracker.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient
{
    public static class DependencyInjectionHelper
    {
        public static IServiceProvider provider { get; set; }
        public static void SetUP()
        {
            var settingsStore = new JsonSettingsStore<UserSettings>(AppPaths.SettingsFile);
            var userSettings = settingsStore.Load();

            var host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureServices((_, services) =>
                    services.AddSingleton<ServiceCache<Velocity>>()
                    .AddSingleton<ServiceCache<Acceleration>>()
                    .AddSingleton<ServiceCache<VelocityCSVModel>>()
                    .AddSingleton<ServiceCache<AccelerationCSVModel>>()
                    .AddSingleton<ServiceCache<PhysicalMotion>>()
                    .AddSingleton<ServiceCache<SetAnalysis>>()
                    .AddSingleton<ServiceCache<SetCSVModel>>()
                    .AddSingleton(settingsStore)
                    .AddSingleton(userSettings)
                    .AddSingleton(new AnalysisSettings() { SmoothingWindow = userSettings.SmoothingWindow, MinRangeOfMotion = userSettings.MinRangeOfMotion })
                    .AddSingleton<ISessionRepository>(new JsonSessionRepository(AppPaths.SessionsFolder))
                    .AddSingleton<FileManager>(sp => new FileManager(sp.GetRequiredService<IEventSystem>())
                    {
                        FolderPath = string.IsNullOrWhiteSpace(userSettings.OutputFolder) ? AppPaths.DefaultExportFolder : userSettings.OutputFolder
                    })
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
            provider = Scope.ServiceProvider;
        }
    }
}
