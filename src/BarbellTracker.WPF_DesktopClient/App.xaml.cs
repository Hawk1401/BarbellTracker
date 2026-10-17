using BarbellTracker.ApplicationCode;
using BarbellTracker.Plugins.Processing;
using BarbellTracker.Plugins.Tracker;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BarbellTracker.WPF_DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            // Bindings use en-US by default, so a german "45,5" would not be accepted
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));

            DependencyInjectionHelper.SetUP();
            var provider = DependencyInjectionHelper.provider;
            var tacker = provider.GetRequiredService<JsonLoader>();
            var Processing1 = provider.GetRequiredService<VelocityToCSVFile>();
            var Processing2 = provider.GetRequiredService<AccelerationToCSVFile>();
            var Processing3 = provider.GetRequiredService<VelocityToAdapterTable>();
            var Processing4 = provider.GetRequiredService<AccelerationToAdapterTable>();
            var Processing5 = provider.GetRequiredService<SetMetricsToCSVFile>();

            
            base.OnStartup(e);
            new MainWindow().Show();


            // Den Rest hier instanziieren !!

           

        }
    }
}
