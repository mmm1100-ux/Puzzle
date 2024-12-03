using Puzzle;
using System.Collections.Generic;

namespace Services
{
    public class SmsService
    {
        public void Send(string receptor, Enums.Enum.SmsTemplate template, string token, string token2, string token3)
        {
            var api = new Kavenegar.KavenegarApi(Setting.Kavenegar);
            api.VerifyLookup(receptor, token, token2, token3, template.ToString()).GetAwaiter();
        }
    }
}
