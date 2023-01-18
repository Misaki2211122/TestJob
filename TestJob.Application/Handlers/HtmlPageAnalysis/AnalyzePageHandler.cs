using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Html.Parser;
using MediatR;
using TestJob.Application.Domains.Requests.HtmlPageAnalysis;
using TestJob.Application.Domains.Responses.HtmlPageAnalysis;

namespace TestJob.Application.Handlers.HtmlPageAnalysis;

public class AnalyzePageHandler : IRequestHandler<AnalyzePageRequest, AnalyzePageResponse>
{
    internal readonly IConfiguration _configuration;
    internal readonly string _strRegex = @"[A-Za-z0-9_\-\+]+@[A-Za-z0-9\-]+\.([A-Za-z]{2,3})(?:\.[a-z]{2})?";
    internal readonly Regex _regex;
    
    public AnalyzePageHandler()
    {
        _configuration = Configuration.Default;
        _regex = new Regex(_strRegex, RegexOptions.None);
    }

    public async Task<AnalyzePageResponse> Handle(AnalyzePageRequest request, CancellationToken cancellationToken)
    {
        var url = Base64Decryption(request.Url_b64); // Расшифрованный URL

        var page = Base64Decryption(request.Page_b64); // Расшифрованная страница

        var context = BrowsingContext.New(_configuration);
        var parser = context.GetService<IHtmlParser>();
        var document = parser.ParseDocument(page); //преобразование HTML-текста в DOM-объект
        var selector = document.QuerySelectorAll(request.Selector); //все элементы по CSS-селектору
        List<string> attribute = selector.Select(x => x.GetAttribute(request.Attribute)).ToList();

        List<string> emails = new List<string>();
        foreach (Match myMatch in _regex.Matches(page))
        {
            if (myMatch.Success) 
                emails.Add(myMatch.Value);
        }
        
        var encryptedTextBytes = System.Convert.FromBase64String(request.Encrypted_text_bytes_b64);
        var keyBytes = System.Convert.FromBase64String(request.Key_bytes_b64);

        var res = DecryptRijndael(encryptedTextBytes, keyBytes);
        
        return new AnalyzePageResponse() {Is_error = 0, Url = url, Elements_count = selector.Length, Elements_attr_list = attribute, Emails_count = emails.Count, Emails_list = emails, Decrypted_plain_text = "decryptedText", Error_code = "200", Error_message = "Success"};
    }

    public string Base64Decryption(string base64String)
    {
        var base64Decryption = System.Convert.FromBase64String(base64String);
        var res = System.Text.Encoding.UTF8.GetString(base64Decryption);
        return res;
    }

    public static string DecryptRijndael(byte[] buffer, byte[] key)
    {
        var rijndael = new RijndaelManaged
        {
            BlockSize = 128,
            IV = new []{Byte.MinValue, Byte.MaxValue, Byte.MinValue, Byte.MinValue, Byte.MinValue, Byte.MinValue, Byte.MinValue, Byte.MinValue, Byte.MinValue, Byte.MinValue,Byte.MinValue,Byte.MinValue},
            KeySize = 192,
            Key = key,
            Mode = CipherMode.ECB
        };

        var transform = rijndael.CreateDecryptor();
        string decrypted;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
                cs.FlushFinalBlock();
                decrypted = Encoding.UTF8.GetString(ms.ToArray());
                cs.Close();
            }
            ms.Close();
        }

        return decrypted;
    }
}