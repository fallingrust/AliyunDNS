using AliyunDns.Core.Beans.Base;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AliyunDns.Core.Util
{
    public class AliyunUtil
    {
        public static async Task<string> GetAsync<Query>(Query query,string baseUrl,string keySec) where Query : AliyunQueryBase
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            var queryStr = BuildQueryString(query.GetQuery());
            var signedStr = Signature("GET", keySec, queryStr);
            var url = $"?Signature={signedStr}&{queryStr}";
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public static string BuildQueryString(Dictionary<string, string> paramDic)
        {
            var sb = new StringBuilder();
            foreach (var kv in paramDic)
            {
                sb.Append('&' + UrlEncode(kv.Key) + "=" + UrlEncode(kv.Value));
            }
            return sb.ToString()[1..];
        }
        public static string UrlEncode(string url)
        {
            return UrlEncoder.Default.Encode(url).Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
        }

        public static string Signature(string method, string secKey, string str)
        {
            str = method.ToUpper() + '&' + UrlEncode("/") + '&' + UrlEncode(str);
            using var alg = new HMACSHA1(Encoding.UTF8.GetBytes(secKey + "&"));           
            return UrlEncode(Convert.ToBase64String(alg.ComputeHash(Encoding.UTF8.GetBytes(str))));
        }
    }
}
