using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TntMPDConverter.Properties;

namespace TntMPDConverter
{
    internal class ConvertStatement
    {
        public void DoConversion()
        {
            string donations;
			using (var reader = new StringReader(ConvertToText(Settings.Default.SourceFile)))
            {
                var state = State.Initialize(reader).NextState();
                donations = ProcessDonations(ref state);
            }
            string filename = Path.Combine(Settings.Default.TargetPath, 
				Path.GetFileNameWithoutExtension(Settings.Default.SourceFile) + ".tntmpd");
            using (StreamWriter writer = File.CreateText(filename))
            {
                writer.WriteLine("[ORGANIZATION]");
                writer.WriteLine("Name=Wycliff e.V.");
                writer.WriteLine("Abbreviation=Wycliff");
                writer.WriteLine("Code=GED");
                writer.Write(donations);
            }
        }

    	private static Encoding CurrentEncoding
    	{
    		get
    		{
				string encodingName = Environment.OSVersion.Platform == PlatformID.Unix ? "UTF-8" : "Windows-1252";
    			return Encoding.GetEncoding(encodingName);
			}
    	}

		private static string ConvertToText(string rtfFileName)
		{
			using (var rtBox = new RichTextBox())
			{
				rtBox.Rtf = File.ReadAllText(rtfFileName, CurrentEncoding);
				return rtBox.Text;
			}
		}

        private static string ProcessDonations(ref State state)
        {
        	var projectNo = 0;
        	var project = state as Project;
			if (project != null)
			{
				state = state.NextState();
				projectNo = project.ProjectNo;
			}
        	var builder = new StringBuilder();
            builder.AppendLine("[GIFTS]");
            builder.AppendLine("\"PEOPLE_ID\",\"ACCT_NAME\",\"DISPLAY_DATE\",\"AMOUNT\",\"DONATION_ID\",\"DESIGNATION\",\"MOTIVATION\"");
            var info1 = new CultureInfo("en-US", false);
            while (state != null)
            {
                if (state is Account)
                {
                    state = state.NextState();
                    var processingDonations = state as ProcessingDonations;
                    while ((processingDonations != null) && processingDonations.StateValid)
                    {
                        var donation = processingDonations.NextDonation;
                        if (donation != null)
                        {
                            builder.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"\"", 
								donation.DonorNo, donation.Donor, donation.Date.ToString("d", info1), 
								donation.Amount.ToString(info1), donation.BookingId , projectNo));
                        }
                    }
                }
                else if (!(state is Ignore))
                {
                    break;
                }
                state = state.NextState();
            }
            return builder.ToString();
        }

/*
        private string ProcessDonors(ref State state)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendLine("[DONORS]");
            builder1.AppendLine("\"PEOPLE_ID\",\"ACCT_NAME\",\"ADDR1\",\"AMOUNT\"");
            CultureInfo info1 = new CultureInfo("en-US");
            while (state != null)
            {
                if (state is Account)
                {
                    state = state.NextState();
                    ProcessingDonations donations1 = state as ProcessingDonations;
                    while ((donations1 != null) && donations1.StateValid)
                    {
                        Donation donation1 = donations1.NextDonation;
                        if (donation1 != null)
                        {
                            builder1.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"", new object[] { donation1.DonorNo, donation1.Donor, donation1.Date.ToString("d", info1), donation1.Amount.ToString(info1) }));
                        }
                    }
                }
                else if (!(state is Ignore))
                {
                    break;
                }
                state = state.NextState();
            }
            return builder1.ToString();
        }
*/

    }
}

