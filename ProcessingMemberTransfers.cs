using System;
using System.Globalization;

namespace TntMPDConverter
{
	public class ProcessingMemberTransfers: ProcessingDonations
	{
		private int m_AccountNo;

		public ProcessingMemberTransfers(int accountNo, Scanner reader): base(reader)
		{
			m_AccountNo = accountNo;
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
						else if (Replacements.IncludeEntry(m_AccountNo, partsOfLine[5]))
						{
							// 30.10.2012	100,00	H	UM 867	Umb. von Frieder Friederich
							// [1]          [2]    [3]  [4]     [5]
							donation = new Donation
							{
								Date = Convert.ToDateTime(partsOfLine[1], cultureInfo),
								Amount = Convert.ToDecimal(partsOfLine[2], cultureInfo),
								Donor = partsOfLine[5],
								DonorNo = (uint)partsOfLine[5].GetHashCode()
							};
							if (partsOfLine[3] == "S")
							{
								donation.Amount = -donation.Amount;
							}
							Replacements.ApplyReplacements(donation);
							if (!string.IsNullOrEmpty(donation.Donor))
								return donation;
							donation = null;
						}
					}
					line = Reader.ReadLine();
				}
			}
		}
	}
}

