using System;
using System.Text.RegularExpressions;

namespace ReceiptClientCLI
{
    public class ReceiptParser
    {
        public string receiptString;

        
        public CardDetails GetCard()
        {
            CardDetails cardDetails = new CardDetails();
            
            Regex trailingDigitsRegex = new Regex(@"[\*|x]{4,8}(\d{4})");
            cardDetails.trailingDigits = trailingDigitsRegex.Matches(receiptString)[0].Groups[1].ToString();
            
            Regex leadingDigitsRegex = new Regex(@"(\d{4,6})[\*|x]{4,8}");
            MatchCollection leadingDigitsMatchCollection = leadingDigitsRegex.Matches(receiptString);
            
            if (leadingDigitsMatchCollection.Count > 0)
            {
                cardDetails.leadingDigits = leadingDigitsMatchCollection[0].Groups[1].ToString();
            }
            else
            {
                cardDetails.leadingDigits = "";
            }

            Regex cardTypeRegex = new Regex(@"(MASTERCARD|Visa Debit)");
            cardDetails.cardType = cardTypeRegex.Matches(receiptString)[0].Groups[1].ToString();
            
            Regex expiryDateRegex = new Regex(@"(\d{2}/\d{2})");
            MatchCollection expiryDateMatchCollection = expiryDateRegex.Matches(receiptString);
            
            if (expiryDateMatchCollection.Count > 0)
            {
                cardDetails.expiryDate = expiryDateMatchCollection[0].Groups[1].ToString();
            }
            else
            {
                cardDetails.expiryDate = "";
            }


            cardDetails.startDate = "";
            
            
            Console.WriteLine(receiptString);
            
            Console.WriteLine(cardDetails.expiryDate);
            
            return cardDetails;
        }
    }
}