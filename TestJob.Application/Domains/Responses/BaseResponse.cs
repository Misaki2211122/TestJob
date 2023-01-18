namespace TestJob.Application.Domains.Responses;

public class BaseResponse
{
    /// <summary>
    /// Успех
    /// </summary>
    public bool Success { get; set; }
        
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorMessage { get; set; }
}