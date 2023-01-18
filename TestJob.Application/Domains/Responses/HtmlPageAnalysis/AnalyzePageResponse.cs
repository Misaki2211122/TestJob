namespace TestJob.Application.Domains.Responses.HtmlPageAnalysis;

public class AnalyzePageResponse
{
    /// <summary>
    /// есть или нет ошибка при обработке запроса
    /// </summary>
    public int Is_error { get; set; }
    
    /// <summary>
    /// текстовый код ошибки
    /// </summary>
    public string Error_code { get; set; }
    
    /// <summary>
    /// для "иных ошибок"
    /// </summary>
    public string Error_message { get; set; }
    
    /// <summary>
    /// количество выбранных по селектору элементов
    /// </summary>
    public int Elements_count { get; set; }
    
    /// <summary>
    /// количество найденных email
    /// </summary>
    public int Emails_count { get; set; }
    
    /// <summary>
    /// URL в открытом виде
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// текст расшифрованный из полученного шифротекста и ключа
    /// </summary>
    public string Decrypted_plain_text { get; set; }
    
    /// <summary>
    /// список выбранных из обнаруженных элементов атрибутов
    /// </summary>
    public List<string> Elements_attr_list { get; set; }
    
    /// <summary>
    /// список email
    /// </summary>
    public List<string> Emails_list { get; set; }
}