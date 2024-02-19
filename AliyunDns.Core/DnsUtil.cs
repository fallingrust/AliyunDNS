using AliyunDns.Core.Beans.Aliyun;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AliyunDns.Core
{
    public class DnsUtil
    {
        static readonly string AliyunCli = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aliyun.exe");
        static DnsUtil()
        {
           
            if(!File.Exists(AliyunCli))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "AliyunDns.Core.Resources.aliyun.aliyun.exe";
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var fs = File.Create(AliyunCli);
                    stream.CopyTo(fs);
                }
            }
        }
        public static void Configure(string id,string sec)
        {
            var args = new string[] { "configure", "set", "--profile", "akProfile", "--mode", "AK", "--region", "cn-hangzhou", "--access-key-id", id, "--access-key-secret", sec };
            using var p = new Process();
            var startInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = AliyunCli,
            };
            foreach (var arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }
            p.StartInfo = startInfo;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Console.WriteLine(e.Data);
            };
            p.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Console.WriteLine(e.Data);
            };
            p.Start();
            p.WaitForExit();
        }       

        public static async Task<QueryRecordResponse?> QueryAsync(string domain)
        {
            //aliyun.exe alidns DescribeDomainRecords  --DomainName chenxuejian.cn
            return await Task.Run(() =>
            {
                try
                {
                    var args = new string[] { "alidns", "DescribeDomainRecords", "--DomainName", domain };
                    using var p = new Process();
                    var startInfo = new ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        FileName = AliyunCli,
                        RedirectStandardOutput = true,
                    };
                    foreach (var arg in args)
                    {
                        startInfo.ArgumentList.Add(arg);
                    }
                    p.StartInfo = startInfo;
                    p.Start();
                    p.WaitForExit();
                    var output = p.StandardOutput.ReadToEnd();

                    return JsonSerializer.Deserialize<QueryRecordResponse>(output, QueryRecordResponseSerializerContext.Default.QueryRecordResponse);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return null;
            });
            
        }

        public static async Task UpdateAsync(string recordId,string rr,string type,string value)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var args = new string[] { "alidns", "UpdateDomainRecord", "--RR", rr, "--RecordId", recordId, "--Type", type, "--Value", value };
                    using var p = new Process();
                    var startInfo = new ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        FileName = AliyunCli,
                        RedirectStandardOutput = true,
                    };
                    foreach (var arg in args)
                    {
                        startInfo.ArgumentList.Add(arg);
                    }
                    p.StartInfo = startInfo;
                    p.Start();
                    p.WaitForExit();
                    var output = p.StandardOutput.ReadToEnd();

                    Console.WriteLine(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return null;
            });
        }
        public static async Task<IPAddress?> GetIPv4AddressAsync()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync("https://ipv4.ip.mir6.com");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _ = IPAddress.TryParse(content, out var ip);
                    return ip;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }


            return null;
        }
        public static async Task<IPAddress?> GetIPv6AddressAsync()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync("https://ipv6.ip.mir6.com");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _ = IPAddress.TryParse(content, out var ip);
                    return ip;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
