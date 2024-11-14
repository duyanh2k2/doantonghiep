using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DoAn.Services
{
    public class SmsService
    {
        private readonly IConfiguration _configuration;
        public SmsService(IConfiguration configuration)
        {
            this._configuration = configuration;
            TwilioClient.Init(_configuration["Twilio:AccountSid"],
                _configuration["Twilio:AuthToken"]);
        }
        public virtual async Task SendSmsAsync(string to, string message)
        {
            var from = new PhoneNumber(_configuration["Twilio:FromNumber"]);
            var toNumber = new PhoneNumber(to);

            var messageOptions = new CreateMessageOptions(toNumber)
            {
                From = from,
                Body = message
            };

            // Sử dụng CreateAsync để gửi tin nhắn bất đồng bộ
            await MessageResource.CreateAsync(messageOptions);
        }
    }
}
