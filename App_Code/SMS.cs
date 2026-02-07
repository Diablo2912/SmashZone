using System;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SmashZone.App_Code
{
    public static class SMS
    {
        public static void SendOTP(string toPhoneE164, string otp)
        {
            if (string.IsNullOrWhiteSpace(toPhoneE164))
                throw new ArgumentException("Phone number is missing.");

            string sid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            string token = ConfigurationManager.AppSettings["TwilioAuthToken"];
            string from = ConfigurationManager.AppSettings["TwilioFrom"];

            TwilioClient.Init(sid, token);

            MessageResource.Create(
                to: new PhoneNumber(toPhoneE164),
                from: new PhoneNumber(from),
                body: $"SmashZone OTP: {otp} (valid for 3 minutes)"
            );
        }
    }
}
