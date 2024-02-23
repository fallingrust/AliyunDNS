using AliyunDns.Core.Beans.Aliyun;
using AliyunDns.Core.Beans.Base;
using Serilog;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AliyunDns.Core.Util
{
    public class AliyunUtil
    {      
        private static HttpClient? _client;
        private static Config? _config;
        public static void Configure(Config config)
        {
            _config = config;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_config.EndPoint)
            };
        }

        public static async Task UpdateAsync()
        {
           
            Log.Information($"\r\n start update domain records");

            if (_config == null)
            {
                Log.Error($"not config~");
            }
            else
            {
                var ipv4 = (await AddressUtil.GetIPv4AddressAsync(_config.V4Url))?.ToString();
                Log.Information($"query public ipv4:{ipv4}");
                var ipv6 = (await AddressUtil.GetIPv6AddressAsync(_config.V6Url))?.ToString();
                Log.Information($"query public ipv6:{ipv6}");

                foreach(var domain in _config.Domains)
                {
                    await UpdateDomain(domain, ipv4, ipv6);
                }

                foreach (var domain in _config.SubDomains)
                {
                    await UpdateSubDomain(domain, ipv4, ipv6);
                }
            }
           
            Log.Information($"stop update domain records\r\n");
        }

        private static async Task UpdateDomain(Domain domain,string? ipv4,string? ipv6)
        {
            if (_config == null) return;
            if (string.IsNullOrWhiteSpace(domain.DomainName)) return;
            var response = await GetAsync(new DescribeDomainRecordsQuery(domain.DomainName, _config.KeyId), DescribeDomainRecordsResponseSerializerContext.Default.DescribeDomainRecordsResponse);
            if (response != null && response.TotalCount > 0 && response.DomainRecords != null && response.DomainRecords.Record != null)
            {

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

                    if (record.Type == "A" && domain.V4Enable)
                    {
                        if (!string.IsNullOrWhiteSpace(ipv4) && record.Value != ipv4)
                        {
                            Log.Information($"update {record.DomainName} {record.RR} {record.Value}->{ipv4}");
                            var query = new UpdateDomainRecordQuery(_config.KeyId)
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
                    else if (record.Type == "AAAA" && domain.V6Enable)
                    {
                        if (!string.IsNullOrWhiteSpace(ipv6) && record.Value != ipv6)
                        {
                            Log.Information($"update {record.DomainName} {record.RR} {record.Value}->{ipv6}");
                            var query = new UpdateDomainRecordQuery(_config.KeyId)
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
            return;
        }
        private static async Task UpdateSubDomain(Domain domain, string? ipv4, string? ipv6)
        {
            if (_config == null) return;
            if (string.IsNullOrWhiteSpace(domain.DomainName)) return;
            var response = await GetAsync(new DescribeSubDomainRecordsQuery(domain.DomainName, _config.KeyId), DescribeSubDomainRecordsResponseSerializerContext.Default.DescribeSubDomainRecordsResponse);
            if (response != null && response.TotalCount > 0 && response.DomainRecords != null && response.DomainRecords.Record != null)
            {

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

                    if (record.Type == "A" && domain.V4Enable)
                    {
                        if (!string.IsNullOrWhiteSpace(ipv4) && record.Value != ipv4)
                        {
                            Log.Information($"update {record.DomainName} {record.RR} {record.Value}->{ipv4}");
                            var query = new UpdateDomainRecordQuery(_config.KeyId)
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
                    else if (record.Type == "AAAA" && domain.V6Enable)
                    {
                        if (!string.IsNullOrWhiteSpace(ipv6) && record.Value != ipv6)
                        {
                            Log.Information($"update {record.DomainName} {record.RR} {record.Value}->{ipv6}");
                            var query = new UpdateDomainRecordQuery(_config.KeyId)
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
                Log.Information($"query {domain.DomainName} records with null or empty data");
            }
            return;
        }
        public static async Task<Response?> GetAsync<Query, Response>(Query query, JsonTypeInfo<Response> jsonTypeInfo)
            where Query : AliyunQueryBase
            where Response : AliyunResponseBase
        {
            if (_client == null) return null;
            if (_config == null) return null;
            var queryStr = BuildQueryString(query.GetQuery());
            var signedStr = Signature("GET", _config.KeySecret, queryStr);
            
            Log.Debug("\r\n----------Signed String ----------");
            Log.Debug(signedStr);
            Log.Debug("----------Signed String ----------\r\n");

            var url = $"?Signature={signedStr}&{queryStr}";

            Log.Debug("\r\n----------Query Url----------");
            Log.Debug(url);
            Log.Debug("----------Query Url ----------\r\n");

            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Log.Debug("\r\n----------API Response ----------");
            Log.Debug(content);
            Log.Debug("----------API Response ----------\r\n");


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
            Log.Debug("\r\n---------- String To Be Signed ----------");
            Log.Debug(str);
            Log.Debug("----------String To Be Signed ----------\r\n");

            using var alg = new HMACSHA1(Encoding.UTF8.GetBytes(secKey + "&"));           
            return UrlEncode(Convert.ToBase64String(alg.ComputeHash(Encoding.UTF8.GetBytes(str))));
        }
    }
}
