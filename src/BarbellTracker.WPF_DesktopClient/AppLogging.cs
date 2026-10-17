using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BarbellTracker.ApplicationCode;
using Serilog;

namespace BarbellTracker.WPF_DesktopClient
{
    /// <summary>File logging to %AppData%\BarbellTracker\logs, one file per day, the last 14 days are kept</summary>
    internal static class AppLogging
    {
        public static string Version =>
            Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0] ?? "0.0.0";

        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    Path.Combine(AppPaths.LogsFolder, "barbelltracker-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // the plugins and services (which do not know the logging library) report errors with Trace
            Trace.Listeners.Add(new SerilogTraceListener());

            Log.Information("BarbellTracker {Version} started", Version);
        }

        public static void Shutdown()
        {
            Log.Information("BarbellTracker stopped");
            Log.CloseAndFlush();
        }
    }

    /// <summary>Forwards System.Diagnostics.Trace messages to the log</summary>
    internal class SerilogTraceListener : TraceListener
    {
        public override void Write(string message) => Log.Information("{Message}", message);

        public override void WriteLine(string message) => Log.Information("{Message}", message);

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    Log.Error("{Message}", message);
                    break;
                case TraceEventType.Warning:
                    Log.Warning("{Message}", message);
                    break;
                default:
                    Log.Information("{Message}", message);
                    break;
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEvent(eventCache, source, eventType, id, args == null || args.Length == 0 ? format : string.Format(format, args));
        }
    }
}
