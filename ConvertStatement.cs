// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TntMPDConverter.Properties;

namespace TntMPDConverter
{
	internal class ConvertStatement
	{
		private Dictionary<uint, Donation> m_Donations = new Dictionary<uint, Donation>();

		public void DoConversion()
		{
			string donations;
			string donors;
			using (var reader = new StringReader(ConvertToText(Settings.Default.SourceFile)))
			{
				var state = State.Initialize(reader).NextState();
				donations = ProcessDonations(ref state);
				donors = ProcessDonors(ref state);
			}
			string filename = Path.Combine(Settings.Default.TargetPath,
				Path.GetFileNameWithoutExtension(Settings.Default.SourceFile) + ".tntmpd");
			using (var writer = new StreamWriter(File.Open(filename, FileMode.Create),
				Encoding.GetEncoding("Windows-1252")))
			{
				writer.WriteLine("[ORGANIZATION]");
				writer.WriteLine("Name=Wycliff e.V.");
				writer.WriteLine("Abbreviation=Wycliff");
				writer.WriteLine("Code=GED");
				writer.WriteLine(donors);
				writer.Write(donations);
			}
		}

		private static bool IsUnix
		{
			get { return Environment.OSVersion.Platform == PlatformID.Unix; }
		}

		private static string ConvertToText(string rtfFileName)
		{
			var rtfConverter = new RtfConverter(rtfFileName);
			return rtfConverter.Text;
		}

		private string ProcessDonations(ref State state)
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
			builder.AppendLine("\"PEOPLE_ID\",\"ACCT_NAME\",\"DISPLAY_DATE\",\"AMOUNT\",\"DONATION_ID\",\"DESIGNATION\",\"MOTIVATION\",\"MEMO\",\"TENDERED_AMOUNT\",\"TENDERED_CURRENCY\"");
			var cultureInfo = new CultureInfo("en-US", false);
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
							builder.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"\",\"{6}\",\"{7}\",\"{8}\"",
								donation.DonorNo, donation.Donor,
								donation.Date.ToString("d", cultureInfo),
								donation.Amount.ToString(cultureInfo),
								donation.BookingId, projectNo,
								donation.Remarks,
								string.IsNullOrEmpty(donation.TenderedCurrency) ? string.Empty : donation.TenderedAmount.ToString(),
								donation.TenderedCurrency));
							// we use m_Donations to track donations without donor address,
							// so it doesn't matter if we override one if one donor has made
							// multiple donations.
							m_Donations[donation.DonorNo] = donation;
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

		private string ProcessDonors(ref State state)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("[DONORS]");
			builder.AppendLine("\"PEOPLE_ID\",\"ACCT_NAME\",\"PERSON_TYPE\"," + 
				"\"LAST_NAME_ORG\",\"ORG_CONTACT_PERSON\",\"FIRST_NAME\",\"MIDDLE_NAME\",\"TITLE\",\"SUFFIX\"," + 
				"\"SP_LAST_NAME\",\"SP_FIRST_NAME\",\"SP_MIDDLE_NAME\",\"SP_TITLE\",\"SP_SUFFIX\"," + 
				"\"ADDR1\",\"ADDR2\",\"ADDR3\",\"ADDR4\",\"CITY\"," + 
				"\"STATE\",\"ZIP\",\"COUNTRY\",\"CNTRY_DESCR\",\"ADDR_CHANGED\"," + 
				"\"PHONE\",\"PHONE_CHANGED\",\"EMAIL\"");
			while (state != null)
			{
				var processDonors = state as ProcessDonors;
				if (processDonors != null)
				{
					var donor = processDonors.NextDonor;
					if (donor != null)
					{
						builder.AppendFormat("\"{0}\",\"{1}\",\"{2}\"," +
							// Person
							"\"{3}\",\"{4}\",\"{5}\",,\"{6}\",," +
							// Spouse
							"\"{7}\",\"{8}\",,\"{9}\",," +
							// Address
							"\"{10}\",,,,\"{11}\",,\"{12}\",\"DE\",\"Germany\",," +
							// Phone
							"\"{13}\",\" \",\"{14}\"",
							donor.DonorNo, donor.Name, donor.PersonType,
							donor.LastName, donor.ContactPerson, donor.FirstName, donor.Title,
							donor.SpouseLastName, donor.SpouseFirstName, donor.SpouseTitle,
							donor.Street, donor.City, donor.Plz,
							donor.CombinedPhoneNo, donor.Email);
						builder.AppendLine();
						m_Donations.Remove(donor.DonorNo);
					}
				}
				else if (!(state is Ignore))
				{
					break;
				}
				state = state.NextState();
			}

//			// Add pseudo-donors for all donations that are left over
//			foreach (var donationKeyValue in m_Donations)
//			{
//				builder.AppendFormat("\"{0}\",\"{1}\",\"O\"," +
//					// Person
//					"\"Unbekannt\",\" \",,," +
//					// Spouse
//					",,,,," +
//					// Address
//					"\" \",,,,\" \",,\" \",\"DE\",\"Germany\",," +
//					// Phone
//					"\" \",\" \"",
//					donationKeyValue.Value.DonorNo, donationKeyValue.Value.Donor);
//				builder.AppendLine();
//			}
			return builder.ToString();
		}
	}
}
