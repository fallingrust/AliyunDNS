using Serilog;
using System.Net;

namespace AliyunDns.Core.Util
{
    public class AddressUtil
    {
        static readonly HttpClient _client;
        static AddressUtil()
        {
            _client = new HttpClient();
        }
        public static async Task<IPAddress?> GetIPv4AddressAsync()
        {
            try
            {                
                var response = await _client.GetAsync("https://ipv4.ip.mir6.com");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _ = IPAddress.TryParse(content, out var ip);
                    return ip;
                }
            }
            catch (Exception ex) { Log.Error(ex, "Unhandled exception"); }


            return null;
        }
        public static async Task<IPAddress?> GetIPv6AddressAsync()
        {
            try
            {
                var response = await _client.GetAsync("https://ipv6.ip.mir6.com");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _ = IPAddress.TryParse(content, out var ip);
                    return ip;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception");
            }

            return null;
        }
    }
}
