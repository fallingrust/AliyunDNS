using System.Text.Json.Serialization;

namespace AliyunDns.Core.Beans.Aliyun
{
    public class UpdateDomainRecordResponse
    {
        public string? RequestId { get; set; }
        public string? RecordId { get; set; }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(UpdateDomainRecordResponse))]
    [JsonSerializable(typeof(IEnumerable<UpdateDomainRecordResponse>))]
    public partial class UpdateDomainRecordResponseSerializerContext : JsonSerializerContext
    {
    }
}
