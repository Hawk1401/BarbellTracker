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
            var host = Host.CreateDefaultBuilder(new string[0])
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
            provider = Scope.ServiceProvider;
        }
    }
}
