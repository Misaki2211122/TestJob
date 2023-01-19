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
        if (request.Selector == "")
            return new AnalyzePageResponse() {Is_error = 1,  Error_code = "406", Error_message = "Empty Selector"};
        
        if (request.Attribute == "")
            return new AnalyzePageResponse() {Is_error = 1,  Error_code = "406", Error_message = "Empty Attribute"};
        
        if (!TryToBase64(request.Url_b64))
            return new AnalyzePageResponse() {Is_error = 1,  Error_code = "406", Error_message = "Can't convert to base64 url"};
        var url = Base64Decryption(request.Url_b64); // Расшифрованный URL
        
        if (!TryToBase64(request.Page_b64))
            return new AnalyzePageResponse() {Is_error = 1,  Error_code = "406", Error_message = "Can't convert to base64 page"};
        var page = Base64Decryption(request.Page_b64); // Расшифрованная страница

        var context = BrowsingContext.New(_configuration);
        var parser = context.GetService<IHtmlParser>();
        var document = parser.ParseDocument(page); //преобразование HTML-текста в DOM-объект
        var selector = document.QuerySelectorAll(request.Selector); //все элементы по CSS-селектору
        List<string> attribute = selector.Select(x => x.GetAttribute(request.Attribute)).ToList();

        List<string> emails = new List<string>(); // лист майлов
        foreach (Match myMatch in _regex.Matches(page))
        {
            if (myMatch.Success) 
                emails.Add(myMatch.Value);
        }
        
        var encryptedTextBytes = System.Convert.FromBase64String(request.Encrypted_text_bytes_b64);
        var keyBytes = System.Convert.FromBase64String(request.Key_bytes_b64);

        var decryptedText = DecryptStringFromBytes_Aes(encryptedTextBytes, keyBytes); // расшифровка сообщения
        
        return new AnalyzePageResponse() {Is_error = 0, Url = url, Elements_count = selector.Length, Elements_attr_list = attribute, Emails_count = emails.Count, Emails_list = emails, Decrypted_plain_text = decryptedText, Error_code = "200", Error_message = "Success"};
    }

    public string Base64Decryption(string base64String)
    {
        var base64Decryption = System.Convert.FromBase64String(base64String);
        var res = System.Text.Encoding.UTF8.GetString(base64Decryption);
        return res;
    }
    
    public bool TryToBase64(string value)
    {
        try
        {
            var result = Convert.FromBase64String(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
       

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create() )
        {
            aesAlg.Key = Key;
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.None;
                
            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                
            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}