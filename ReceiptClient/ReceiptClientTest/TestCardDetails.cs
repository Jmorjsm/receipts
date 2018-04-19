using System;
using Xunit;
using ReceiptClientCLI;

namespace ReceiptClientTest
{
    public class TestCardDetails
    {
        private ReceiptParser _receiptParser;

        public TestCardDetails()
        {
            _receiptParser = new ReceiptParser();
        }

        [Fact]
        public void CardTest1()
        {
            _receiptParser.receiptString = @"MASTERCARD
            Auth Code: 759830
            Merchant ID: 887
            Account Number: ************3456 Expiry: 08/15
            NO CARDHOLDER VERIFICATION";
            
            CardDetails cardDetails = _receiptParser.GetCard();
            
            Assert.Equal("3456", cardDetails.trailingDigits);
            Assert.Equal("", cardDetails.leadingDigits);
            Assert.Equal("MASTERCARD", cardDetails.cardType);
            Assert.Equal("08/15", cardDetails.expiryDate);
            Assert.Equal("", cardDetails.startDate);
            
        }
        [Fact]
        public void CardTest2()
        {
            _receiptParser.receiptString = @"Please debit my account 
            Visa Debit 
            8768xxxxxxxx7682
            Auth Code: 87ff6f";
            
            CardDetails cardDetails = _receiptParser.GetCard();
            
            Assert.Equal("7682", cardDetails.trailingDigits);
            Assert.Equal("8768", cardDetails.leadingDigits);
            Assert.Equal("Visa Debit", cardDetails.cardType);
            Assert.Equal("", cardDetails.expiryDate);
            Assert.Equal("", cardDetails.startDate);
        }
    }
}