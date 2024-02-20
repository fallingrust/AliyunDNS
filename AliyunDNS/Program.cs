using AliyunDns.Core;
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
            foreach( var arg in args )
            {
                Log.Information($"start arg:{arg}");
            }
            DnsUtil.Configure(args[0], args[1]);
            _timer = new Timer(async obj =>
            {
                Log.Information($"start query records");
                var response =  await DnsUtil.QueryAsync(args[2]);

                if (response != null && response.TotalCount > 0 && response.DomainRecords != null && response.DomainRecords.Record != null)
                {
                    var ipv4 = (await DnsUtil.GetIPv4AddressAsync())?.ToString();
                    Log.Information($"query public ipv4:{ipv4}");
                    var ipv6 = (await DnsUtil.GetIPv6AddressAsync())?.ToString();
                    Log.Information($"query public ipv6:{ipv6}");                   
                    foreach (var record in response.DomainRecords.Record)
                    {
                        if (string.IsNullOrWhiteSpace(record.RecordId) || string.IsNullOrWhiteSpace(record.Type) || string.IsNullOrWhiteSpace(record.RR))
                        {
                            Log.Information($"{record.DomainName} params is empty~");
                            continue;
                        }
                        if (record.Locked) 
                        {
                            Log.Information($"{record.DomainName} {record.RR} is locked~");
                            continue;
                        }
                        if (record.Status?.ToLower() != "enable")
                        {
                            Log.Information($"{record.DomainName} {record.RR} is not enable~");
                            continue;
                        }
                        
                        if (record.Type == "A")
                        {
                            if (!string.IsNullOrWhiteSpace(ipv4) && record.Value != ipv4)
                            {
                                Log.Information($"update {record.DomainName} {record.RR} {record.Value}->{ipv4}");
                                await DnsUtil.UpdateAsync(record.RecordId, record.RR, record.Type, ipv4);
                            }
                            else
                            {
                                Log.Information($" {record.DomainName} {record.RR}  not changed");
                            }
                        }
                        else if (record.Type == "AAAA")
                        {
                            if (!string.IsNullOrWhiteSpace(ipv6) && record.Value != ipv6)
                            {
                                Log.Information($"update {record.DomainName} {record.RR} {record.Value}->{ipv6}");
                                await DnsUtil.UpdateAsync(record.RecordId, record.RR, record.Type, ipv6);
                            }
                            else
                            {
                                Log.Information($" {record.DomainName} {record.RR}  not changed");
                            }
                        }
                    }
                }
                else
                {
                    Log.Information($"query records with null or empty data");
                }
            }, null, 0, 600 * 1000);
           
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) break;
            }
        }       
    }
}
