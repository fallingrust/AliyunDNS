using AliyunDns.Core.Beans.Base;

namespace AliyunDns.Core.Beans.Aliyun
{
    public class DescribeDomainRecordsQuery(string domainName, string action, string accessKeyId) : AliyunQueryBase(action, accessKeyId)
    {
        public string DomainName { get; set; } = domainName;

        public long PageNumber { get; set; } = 1;

        public long PageSize { get; set; } = 20;
        public override Dictionary<string, string> GetQuery()
        {
            var dic = GetParamsDictionary();
            dic.Add("DomainName", DomainName);
            dic.Add("PageNumber", PageNumber.ToString());
            dic.Add("PageSize", PageSize.ToString());
            return dic;
        }           
    }
}
