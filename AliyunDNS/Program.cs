using AliyunDns.Core.Util;
using Serilog;

namespace AliyunDNS
{
    internal class Program
    {
        private static Timer? _timer;
        static void Main(string[] args)
        {


            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt",
                              rollingInterval: RollingInterval.Day,
                              rollOnFileSizeLimit: true)
                .CreateLogger();
            Log.Information($"aliyun dns start with args({args.Length})");
            foreach (var arg in args)
            {
                Log.Information($"start arg:{arg}");
            }
            AliyunUtil.Configure("https://alidns.cn-hangzhou.aliyuncs.com", args[0], args[1], args[2]);
            _timer = new Timer(async obj =>
            {
                await AliyunUtil.UpdateAsync();
            }, null, 0, 600 * 1000);

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) break;
            }
        }
    }
}
