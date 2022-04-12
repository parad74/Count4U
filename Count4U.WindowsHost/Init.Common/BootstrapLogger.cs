using System;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Targets;

namespace Count4U
{
    public class BootstrapLogger
    {
        public const string LogDirName = "Logs";
        public string ConfigureLogging()
        {
            FileTarget target = LogManager.Configuration.FindTargetByName("logfile") as FileTarget;
            if (target != null)
            {
                //change path only in RELEASE mode
#if DEBUG
                return GetLogDir();
#else              
                string logPath = GetLogDir();
                target.FileName = Path.Combine(logPath, "log.txt");
                target.ArchiveFileName = String.Format(@"{0}\log.{{#####}}.txt", logPath);                

                return logPath;
#endif
            }

            return string.Empty;
        }

        public static string GetLogDir()
        {
            string dir = string.Empty;
#if DEBUG
            dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), LogDirName);
#else
                string path = FileSystem.CommonCount4UFolder();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string logDirName = LogDirName;
                string logDirPath = Path.Combine(path, logDirName);

            dir = logDirPath;
#endif

            if (!string.IsNullOrWhiteSpace(dir))
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            return dir;
        }
    }
}