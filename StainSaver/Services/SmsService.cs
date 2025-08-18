using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class SmsService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;
    private readonly ILogger<SmsService> _logger;

    public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
    {
        _accountSid = configuration["Twilio:AccountSid"];
        _authToken = configuration["Twilio:AuthToken"];
        _fromNumber = configuration["Twilio:FromNumber"];
        _logger = logger;

        TwilioClient.Init(_accountSid, _authToken);
    }

    public async Task SendSmsAsync(string to, string message)
    {
        try
        {
            _logger.LogInformation("Attempting to send SMS to {PhoneNumber}", to);

            var result = await MessageResource.CreateAsync(
                to: new PhoneNumber(to),
                from: new PhoneNumber(_fromNumber),
                body: message
            );

            _logger.LogInformation("SMS sent. SID: {Sid}, Status: {Status}", result.Sid, result.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", to);
        }
    }
}
