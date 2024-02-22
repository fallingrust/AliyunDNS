using AliyunDns.Core.Beans.Aliyun;
using AliyunDns.Core.Beans.Base;
using Serilog;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AliyunDns.Core.Util
{
    public class AliyunUtil
    {
        private static string _baseUrl = string.Empty;
        private static string _keySec = string.Empty;
        private static string _keyId = string.Empty;
        private static string _domain = string.Empty;
        private static HttpClient? _client;       
        public static void Configure(string baseUrl,string keyId, string keySec,string domain)
        {
            _baseUrl = baseUrl;
            _keySec = keySec;
            _keyId = keyId;
            _domain = domain;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        public static async Task UpdateAsync()
        {
            Log.Information($"start describe domain records");
            var response = await GetAsync(new DescribeDomainRecordsQuery(_domain, _keyId), DescribeDomainRecordsResponseSerializerContext.Default.DescribeDomainRecordsResponse);
            if (response != null && response.TotalCount > 0 && response.DomainRecords != null && response.DomainRecords.Record != null)
            {
                var ipv4 = (await AddressUtil.GetIPv4AddressAsync())?.ToString();
                Log.Information($"query public ipv4:{ipv4}");
                var ipv6 = (await AddressUtil.GetIPv6AddressAsync())?.ToString();
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
                            var query = new UpdateDomainRecordQuery(_keyId)
                            {
                                RecordId = record.RecordId,
                                Type = record.Type,
                                RR = record.RR,
                                Value = ipv4
                            };
                            await GetAsync(query, UpdateDomainRecordResponseSerializerContext.Default.UpdateDomainRecordResponse);
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
                            var query = new UpdateDomainRecordQuery(_keyId)
                            {
                                RecordId = record.RecordId,
                                Type = record.Type,
                                RR = record.RR,
                                Value = ipv6
                            };
                            await GetAsync(query, UpdateDomainRecordResponseSerializerContext.Default.UpdateDomainRecordResponse);
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
        }
        public static async Task<Response?> GetAsync<Query, Response>(Query query, JsonTypeInfo<Response> jsonTypeInfo)
            where Query : AliyunQueryBase
            where Response : AliyunResponseBase
        {
            if (_client == null) return null;
            var queryStr = BuildQueryString(query.GetQuery());
            var signedStr = Signature("GET", _keySec, queryStr);
            var url = $"?Signature={signedStr}&{queryStr}";
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Log.Information(content);
            return JsonSerializer.Deserialize(content, jsonTypeInfo);
        }

        public static string BuildQueryString(SortedDictionary<string, string> paramDic)
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
            return System.Net.WebUtility.UrlEncode(url).Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
        }

        public static string Signature(string method, string secKey, string str)
        {
            str = method.ToUpper() + '&' + UrlEncode("/") + '&' + UrlEncode(str);
            Log.Information($"to be signed string :{str}");
            using var alg = new HMACSHA1(Encoding.UTF8.GetBytes(secKey + "&"));           
            return UrlEncode(Convert.ToBase64String(alg.ComputeHash(Encoding.UTF8.GetBytes(str))));
        }
    }
}
