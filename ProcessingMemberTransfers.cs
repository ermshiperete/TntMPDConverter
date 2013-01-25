using System;
using System.Globalization;

namespace TntMPDConverter
{
	public class ProcessingMemberTransfers: ProcessingDonations
	{
		public ProcessingMemberTransfers(Scanner reader): base(reader)
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
						string[] partsOfLine = line.Split(new[] { '\t' });
						if (partsOfLine.Length != 6)
						{
							if (IsEndOfDonation(line, partsOfLine[0]))
								return donation;
						}
						else
						{
							// 30.10.2012	100,00	H	UM 867	Umb. von Frieder Friederich
							// [1]          [2]    [3]  [4]     [5]
							donation = new Donation
							{
								Date = Convert.ToDateTime(partsOfLine[1], cultureInfo),
								Amount = Convert.ToDecimal(partsOfLine[2], cultureInfo),
								DonorNo = Convert.ToInt32(partsOfLine[4].Substring(3))
							};
							if (partsOfLine[3] == "S")
							{
								donation.Amount = -donation.Amount;
							}
							donation.Donor = partsOfLine[5];
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

