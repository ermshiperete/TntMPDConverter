namespace TntMPDConverter
{
	using System;
	using System.Globalization;

	internal class ProcessingOtherProceeds : ProcessingDonations
	{
		public ProcessingOtherProceeds(Scanner reader) : base(reader)
		{
		}

		public override Donation NextDonation
		{
			get
			{
				var info1 = new CultureInfo("de-DE");
				Donation donation1 = null;
				string text1 = Reader.ReadLine();
				while (true)
				{
					if (text1.Trim() != "")
					{
						string[] textArray1 = text1.Split(new[] { '\t' });
						if (textArray1.Length != 6)
						{
							if (!textArray1[0].StartsWith("Projekt"))
							{
								Reader.UnreadLine(text1);
								IsValid = false;
								return donation1;
							}
						}
						else
						{
							donation1 = new Donation { DonorNo = 998, Date = Convert.ToDateTime(textArray1[1], info1), Amount = Convert.ToDecimal(textArray1[2], info1) };
							if (textArray1[3] == "S")
							{
								donation1.Amount = -donation1.Amount;
							}
							donation1.Donor = textArray1[5];
							Properties.Settings.Default.BookingId++;
							donation1.BookingId = Properties.Settings.Default.BookingId.ToString();
							return donation1;
						}
					}
					text1 = Reader.ReadLine();
				}
			}
		}

	}
}

