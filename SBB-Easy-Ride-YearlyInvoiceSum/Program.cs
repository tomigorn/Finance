using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        //TODO replace with your values
        string myMail = "my@email.com";
        string myPassword = "superSecure2faGeneratedPassword";
        int year = 2022;
        string directoryPath = @"C:\Users\User\Documents\Taxes\2022\Invoices\2022-EasyRide";
        string senderEmail = "sbb.feedback@fairtiq.com";
        //TODO end of reaplce your own values.
        //Don't touch after this!

        decimal totalAmountForYear = 0;
        Directory.CreateDirectory(directoryPath);

        // Create a temporary CSV file.
        var csvFilePath = System.IO.Path.Combine(directoryPath, "temp.csv");
        using (var csvWriter = new StreamWriter(csvFilePath))
        {
            // Connect to the IMAP server.
            using (var client = new ImapClient())
            {
                client.Connect("imap-mail.outlook.com", 993, true);

                // Authenticate to the IMAP server.
                client.Authenticate(myMail, myPassword);
                /* If you have 2FA in outlook generate an "App Password"
                    Here's a general way to create a new app password in Outlook.com:
                    1. Sign in to your account.
                    2. Go to your security settings page (you might be asked to sign in again).
                    3. Find the "Two-step verification" section, and then, in the "App passwords" section, click on "Create a new app password".
                    4. The new app password appears on your screen -> use it as myPassword
                */

                // Select the Inbox folder.
                var inbox = client.Inbox;
                inbox.Open(MailKit.FolderAccess.ReadOnly);

                // Search for emails from the specific company.
                var query = SearchQuery.FromContains(senderEmail)
                    .And(SearchQuery.DeliveredAfter(new DateTime(year, 1, 1)))
                    .And(SearchQuery.DeliveredBefore(new DateTime(year+1, 1, 1)));
                var uids = inbox.Search(query);

                foreach (var uid in uids)
                {
                    var message = inbox.GetMessage(uid);

                    // Process each attachment
                    foreach (var attachment in message.Attachments.OfType<MimePart>())
                    {
                        // We're only interested in PDF attachments.
                        if (!attachment.FileName.EndsWith(".pdf"))
                            continue;

                        var fileName = System.IO.Path.Combine(directoryPath, attachment.FileName);

                        // Download each attachment and save it to the specific folder.
                        using (var stream = File.Create(fileName))
                        {
                            attachment.Content.DecodeTo(stream);
                        }

                        var fileParts = attachment.FileName.Split('_');
                        if (fileParts.Length == 3)
                        {
                            var newFileName = $"{fileParts[1]}-EasyRide-{fileParts[2]}";
                            var newFilePath = System.IO.Path.Combine(directoryPath, newFileName);
                            File.Copy(fileName, newFilePath);
                            File.Delete(fileName);

                            // After renaming the file, read the content.
                            string pdfText = ReadPdf(newFilePath);

                            // Extract the total amount from the PDF text.
                            decimal totalAmount = ExtractTotalAmount(pdfText);

                            // Write the email date and amount to the CSV file.
                            csvWriter.WriteLine($"{message.Date:dd.MM.yyyy},{totalAmount},CHF");

                            // Add the total amount to the total for the year.
                            totalAmountForYear += totalAmount;
                        }
                    }
                }

                // Disconnect from the IMAP server.
                client.Disconnect(true);
            }
        }

        // Rename the CSV file to the total amount.
        var newCsvFilePath = System.IO.Path.Combine(directoryPath, $"{totalAmountForYear}CHF.csv");
        File.Move(csvFilePath, newCsvFilePath);
    }

    // Method to read PDF content.
    private static string ReadPdf(string filePath)
    {
        using (PdfReader reader = new PdfReader(filePath))
        {
            StringBuilder textBuilder = new StringBuilder();

            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                string text = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                textBuilder.AppendLine(text);
            }

            return textBuilder.ToString().Trim();
        }
    }

    // Method to extract total amount from PDF text.
    private static decimal ExtractTotalAmount(string pdfText)
    {
        var match = Regex.Match(pdfText, @"Total amount charged CHF ([+-]?\d+(?:\.\d+)?)");
        if (match.Success)
        {
            string amountString = match.Groups[1].Value;
            if (decimal.TryParse(amountString, out decimal amount))
            {
                return amount;
            }
        }

        return 0;
    }



    private static decimal OLD(string pdfText)
    {
        var match = Regex.Match(pdfText, @"Total amount charged CHF (\d+\.\d+)");
        if (match.Success)
        {
            return decimal.Parse(match.Groups[1].Value);
        }

        return 0;
    }
}
