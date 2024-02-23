using AliyunDns.Core;
using AliyunDns.Core.Util;
using Serilog;
using System.Text.Json;

namespace AliyunDNS
{
    internal class Program
    {
        private static Timer? _timer;
        static void Main(string[] args)
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logDir, "log.txt"),
                              rollingInterval: RollingInterval.Day,
                              rollOnFileSizeLimit: true)
                .CreateLogger();
            Log.Information($"aliyun dns start with args({args.Length})");
            foreach (var arg in args)
            {
                Log.Information($"start arg:{arg}");
            }
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            Config? config = null;
            if(!File.Exists(configPath) )
            {
                using var sw = new StreamWriter(configPath);
                config = new Config();
                config.Domains.Add(new Domain());
                config.SubDomains.Add(new Domain());
                var configJson = JsonSerializer.Serialize(config, ConfigSerializerContext.Default.Config);
                sw.WriteLine(configJson);
            }
            else
            {
                using var sr = new StreamReader(configPath);
                config = JsonSerializer.Deserialize(sr.ReadToEnd(), ConfigSerializerContext.Default.Config);
            }

            if (config != null && !string.IsNullOrWhiteSpace(config.KeyId) && !string.IsNullOrWhiteSpace(config.KeySecret))
            {
                AliyunUtil.Configure(config);
                _timer = new Timer(async obj =>
                {
                    await AliyunUtil.UpdateAsync();
                }, null, 0, config.Interval * 1000);
            }            
           
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) break;
            }
        }
    }
}
