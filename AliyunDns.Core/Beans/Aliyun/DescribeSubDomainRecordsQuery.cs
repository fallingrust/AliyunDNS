using AliyunDns.Core.Beans.Base;

namespace AliyunDns.Core.Beans.Aliyun
{
    public class DescribeSubDomainRecordsQuery(string subDomain, string accessKeyId, string action = "DescribeSubDomainRecords") : AliyunQueryBase(accessKeyId, action)
    {
        public string SubDomain { get; set; } = subDomain;

        public long PageNumber { get; set; } = 1;

        public long PageSize { get; set; } = 100;
        public override SortedDictionary<string, string> GetQuery()
        {
            var dic = GetParamsDictionary();
            dic.Add("SubDomain", SubDomain);
            dic.Add("PageNumber", PageNumber.ToString());
            dic.Add("PageSize", PageSize.ToString());
            return dic;
        }
    }
}
