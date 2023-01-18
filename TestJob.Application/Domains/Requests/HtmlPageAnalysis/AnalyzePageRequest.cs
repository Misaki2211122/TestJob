using System.Text.Json;
using MediatR;
using Newtonsoft.Json;
using TestJob.Application.Domains.Responses.HtmlPageAnalysis;

namespace TestJob.Application.Domains.Requests.HtmlPageAnalysis;

public class AnalyzePageRequest : IRequest<AnalyzePageResponse>
{
    [JsonRequired]
    public string Selector { get; set; }
    [JsonRequired]
    public string Attribute { get; set; }
    [JsonRequired]
    public string Url_b64 { get; set; }
    [JsonRequired]
    public string Encrypted_text_bytes_b64 { get; set; }
    [JsonRequired]
    public string Key_bytes_b64 { get; set; }
    [JsonRequired]
    public string Page_b64 { get; set; }
}