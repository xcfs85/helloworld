namespace Pindou.Infrastructure.ExternalServices.Sms;

public class SmsResult
{
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface ISmsService
{
    Task<SmsResult> SendCodeAsync(string phone, string code, string scene);
    Task<SmsResult> SendAsync(string phone, string templateCode, Dictionary<string, string> templateParams);
}
