using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using BarbellTracker.ApplicationCode;
using BarbellTracker.Plugins.Processing;
using BarbellTracker.Plugins.Tracker;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BarbellTracker.WPF_DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool errorDialogOpen;

        protected override void OnStartup(StartupEventArgs e)
        {
            AppLogging.Init();
            RegisterGlobalExceptionHandlers();

            // Bindings use en-US by default, so a german "45,5" would not be accepted
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));

            DependencyInjectionHelper.SetUP();
            var provider = DependencyInjectionHelper.provider;

            // the plugins register themselves at the plugin manager when they are created
            provider.GetRequiredService<JsonLoader>();
            provider.GetRequiredService<VelocityToCSVFile>();
            provider.GetRequiredService<AccelerationToCSVFile>();
            provider.GetRequiredService<VelocityToAdapterTable>();
            provider.GetRequiredService<AccelerationToAdapterTable>();
            provider.GetRequiredService<SetMetricsToCSVFile>();

            base.OnStartup(e);
            new MainWindow().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AppLogging.Shutdown();
            base.OnExit(e);
        }

        /// <summary>
        /// Nothing may disappear silently: every unexpected error is written to the log and the user is told where to find it.
        /// </summary>
        private void RegisterGlobalExceptionHandlers()
        {
            DispatcherUnhandledException += (_, args) =>
            {
                Log.Error(args.Exception, "Unhandled exception on the UI thread");
                ShowError(args.Exception);
                args.Handled = true; // the application stays usable, the failed action is lost
            };

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                Log.Fatal(args.ExceptionObject as Exception, "Unhandled exception, the application has to close (terminating: {Terminating})", args.IsTerminating);
                Log.CloseAndFlush();
            };

            TaskScheduler.UnobservedTaskException += (_, args) =>
            {
                Log.Error(args.Exception, "Unobserved task exception");
                args.SetObserved();
            };
        }

        private static void ShowError(Exception exception)
        {
            if (errorDialogOpen)
            {
                return; // an error loop must not stack dialogs
            }

            errorDialogOpen = true;
            try
            {
                MessageBox.Show(
                    $"Es ist ein unerwarteter Fehler aufgetreten:\n\n{exception.Message}\n\nDetails stehen in der Log-Datei:\n{ApplicationCode.AppPaths.LogsFolder}",
                    "BarbellTracker", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                errorDialogOpen = false;
            }
        }
    }
}
