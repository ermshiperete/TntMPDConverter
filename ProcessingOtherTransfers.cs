using System;
using System.Globalization;

namespace TntMPDConverter
{
	/// <summary>
	/// Process transfers from other organizations
	/// </summary>
	public class ProcessingOtherTransfers: ProcessingDonations
	{
		public ProcessingOtherTransfers(Scanner reader): base(reader)
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
						if (partsOfLine.Length != 8)
						{
							if (IsEndOfDonation(line, partsOfLine[0]))
								return donation;
						}
						else
						{
							// 23.11.2012	US$	209,64	162,20	H	8021	SWZ Member gift
							// [1]          [2] [3]     [4]    [5]  [6]     [7]
							donation = new Donation
							{
								Date = Convert.ToDateTime(partsOfLine[1], cultureInfo),
								Amount = Convert.ToDecimal(partsOfLine[4], cultureInfo),
								Donor = partsOfLine[7],
								Remarks = string.Format("Netto; {0} {1}", partsOfLine[2],
									partsOfLine[3])
							};
							if (partsOfLine[5] == "S")
							{
								donation.Amount = -donation.Amount;
							}
							donation.DonorNo = (uint)donation.Donor.GetHashCode();
							Replacements.ApplyReplacements(donation);
							return donation;
						}
					}
					line = Reader.ReadLine();
				}
			}
		}
	}
}

