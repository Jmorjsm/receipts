using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using Newtonsoft.Json;

namespace ReceiptClientCLI
{
    class Program
    {

        static void GetEmails(CardDetails cardDetails)
        {
            WebRequest request = WebRequest.Create("http://localhost:8080/cardDetails?" + cardDetails.GetRequestString());

            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            
            StreamReader reader = new StreamReader(dataStream);

            Console.WriteLine(reader.ReadToEnd());
            
            response.Close();

        }
        
        

        static void Main(string[] args)
        {
            ReceiptParser receiptParser = new ReceiptParser();

            receiptParser.receiptString = @"MASTERCARD
            Auth Code: 759830
            Merchant ID: 887
            Account Number: ************3456 Expiry: 08/15
            NO CARDHOLDER VERIFICATION";

            CardDetails cardDetails = receiptParser.GetCard();
            
            
            GetEmails(cardDetails);
            Console.WriteLine("Would you like to enter a new email for this card(y/n)?");
            
            string response = Console.ReadLine();
            if (response == "y")
            {
                Console.WriteLine("Enter new email address: ");
                string newEmail = Console.ReadLine();
                cardDetails.email = newEmail;
                HttpWebRequest putRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/cardDetails");

                putRequest.ContentType = "text/json";
                putRequest.Method = "PUT";

                string json = JsonConvert.SerializeObject(cardDetails);

                Stream putRequestStream = putRequest.GetRequestStream();
                StreamWriter putStreamWriter = new StreamWriter(putRequestStream);

                putStreamWriter.Write(json);
                putStreamWriter.Close();
                
                HttpWebResponse putResponse = (HttpWebResponse)putRequest.GetResponse();

                if (putResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Success!");
                    GetEmails(cardDetails);
                }
            }

        }
    }
}