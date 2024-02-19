using AliyunDns.Core;

namespace AliyunDNS
{
    internal class Program
    {
        private static Timer? _timer;
        static void Main(string[] args)
        {
            //aliyun configure set --profile akProfile --mode AK --region cn - hangzhou --access-key-id LTAI5tL6AVoa4UtNQcxKtdJF --access-key-secret TyCLI6LOLnc8gMfkcJg2DundIegbYz
            DnsUtil.Configure("LTAI5tL6AVoa4UtNQcxKtdJF", "TyCLI6LOLnc8gMfkcJg2DundIegbYz");
            _timer = new Timer(async obj =>
            {
                var response =  await DnsUtil.QueryAsync("chenxuejian.cn");

                if (response != null && response.TotalCount > 0 && response.DomainRecords != null && response.DomainRecords.Record != null)
                {
                    var ipv4 = (await DnsUtil.GetIPv4AddressAsync())?.ToString();
                    var ipv6 = (await DnsUtil.GetIPv6AddressAsync())?.ToString();
                    Console.WriteLine(ipv4);
                    Console.WriteLine(ipv6);
                    foreach (var record in response.DomainRecords.Record)
                    {
                        if (record.Locked) continue;
                        if (record.Status?.ToLower() != "enable") continue;
                        if (string.IsNullOrWhiteSpace(record.RecordId) || string.IsNullOrWhiteSpace(record.Type) || string.IsNullOrWhiteSpace(record.RR)) continue;
                        if (record.Type == "A")
                        {
                            if (!string.IsNullOrWhiteSpace(ipv4)&& record.Value != ipv4)
                            {
                                await DnsUtil.UpdateAsync(record.RecordId, record.RR, record.Type, ipv4);
                            }
                        }
                        else if (record.Type == "AAAA")
                        {
                            if (!string.IsNullOrWhiteSpace(ipv6) && record.Value != ipv6)
                            {
                                await DnsUtil.UpdateAsync(record.RecordId, record.RR, record.Type, ipv6);
                            }
                        }
                    }
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
