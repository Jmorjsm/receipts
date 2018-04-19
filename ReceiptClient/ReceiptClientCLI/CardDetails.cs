using System;
using Newtonsoft.Json;
namespace ReceiptClientCLI
{
    public class CardDetails
    {
        public string trailingDigits;
        public string leadingDigits;
        public string cardType;
        public string expiryDate;
        public string startDate;
        public string email;
        
        public string GetRequestString()
        {
            string requestString = "";
            requestString += "trailingDigits=" + trailingDigits;
            if (leadingDigits != "")
            {
                requestString += "&leadingDigits=" + leadingDigits;
            }

            if (cardType != "")
            {
                requestString += "&cardType=" + cardType;
            }

            if (expiryDate != "")
            {
                requestString += "&expiryDate=" + expiryDate;
            }

            return requestString;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}