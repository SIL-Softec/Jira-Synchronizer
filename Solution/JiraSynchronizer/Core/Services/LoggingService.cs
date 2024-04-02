using JiraSynchronizer.Core.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Services;

public class LoggingService
{
    // Pfad zum Logfile
    private readonly static string path = @".\log.txt";

    public void Log(LogCategory category, string message)
    {
        DateTime now = DateTime.Now;
        if (category == LogCategory.LogfileInitialized)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine($"[{now}]\t[{(int)category}]\t{message}");
                }
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine($"[{now}]\t[{(int)category}]\t{message}");
            }
        }

    }
}
