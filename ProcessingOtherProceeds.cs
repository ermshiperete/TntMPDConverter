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
				var cultureInfo = new CultureInfo("de-DE");
				Donation donation = null;
				string line = Reader.ReadLine();
				while (true)
				{
					if (line.Trim() != "")
					{
						string[] textArray1 = line.Split(new[] { '\t' });
						if (textArray1.Length != 6)
						{
							if (IsEndOfDonation(line, textArray1[0]))
								return donation;
						}
						else
						{
							donation = new Donation { DonorNo = 998, Date = Convert.ToDateTime(textArray1[1], cultureInfo), Amount = Convert.ToDecimal(textArray1[2], cultureInfo) };
							if (textArray1[3] == "S")
							{
								donation.Amount = -donation.Amount;
							}
							donation.Donor = textArray1[5];
							ApplyReplacements(donation);
							return donation;
						}
					}
					line = Reader.ReadLine();
				}
			}
		}
	}
}

