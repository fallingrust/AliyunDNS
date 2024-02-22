using AliyunDns.Core.Beans.Base;

namespace AliyunDns.Core.Beans.Aliyun
{
    public class UpdateDomainRecordQuery(string accessKeyId, string action = "UpdateDomainRecord") : AliyunQueryBase(action, accessKeyId)
    {
        public string RecordId { get; set; } = "";
        public string RR { get; set; } = "";
        public string Type { get; set; } = "";
        public string Value { get; set; } = "";

        public override SortedDictionary<string, string> GetQuery()
        {
            var dic = GetParamsDictionary();
            dic.Add("RecordId", RecordId);
            dic.Add("RR", RR);
            dic.Add("Type", Type);
            dic.Add("Value", Value);
            return dic;
        }
    }
}
