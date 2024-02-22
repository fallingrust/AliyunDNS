using AliyunDns.Core.Beans.Base;

namespace AliyunDns.Core.Beans.Aliyun
{
    public class DescribeDomainRecordsQuery(string domainName,  string accessKeyId,  string action = "DescribeDomainRecords") : AliyunQueryBase(action, accessKeyId)
    {
        public string DomainName { get; set; } = domainName;

        public long PageNumber { get; set; } = 1;

        public long PageSize { get; set; } = 100;
        public override SortedDictionary<string, string> GetQuery()
        {
            var dic = GetParamsDictionary();
            dic.Add("DomainName", DomainName);
            dic.Add("PageNumber", PageNumber.ToString());
            dic.Add("PageSize", PageSize.ToString());
            return dic;
        }           
    }
}
