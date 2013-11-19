// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System;
using System.Globalization;

namespace TntMPDConverter
{
	/// <summary>
	/// Process transfers from other organizations
	/// </summary>
	public class ProcessingOtherTransfers: ProcessingDonations
	{
		private int m_AccountNo;

		public ProcessingOtherTransfers(int accountNo, Scanner reader): base(reader)
		{
			m_AccountNo = accountNo;
		}

		protected override int NumberOfFields { get { return 8; }}
		protected override int DonorIndex { get { return 7; }}

		protected override bool IsContinuationLine(string[] partsOfLine)
		{
			return string.IsNullOrEmpty(partsOfLine[1]) || string.IsNullOrEmpty(partsOfLine[4]);
		}

		protected override Donation CreateDonation(string[] partsOfLine, CultureInfo cultureInfo)
		{
			if (Replacements.ExcludeEntry(m_AccountNo, partsOfLine[7]))
				return null;

			// 23.11.2012	US$	209,64	162,20	H	8021	SWZ Member gift
			// [1]          [2] [3]     [4]    [5]  [6]     [7]
			var donation = new Donation
			{
				Date = Convert.ToDateTime(partsOfLine[1], cultureInfo),
				Amount = Convert.ToDecimal(partsOfLine[4], cultureInfo),
				Donor = partsOfLine[7],
				Remarks = string.Format("Netto; {0} {1}", partsOfLine[2],
					partsOfLine[3]),
				TenderedAmount = Convert.ToDecimal(partsOfLine[3], cultureInfo),
				TenderedCurrency = partsOfLine[2] == "US$" ? "USD" : partsOfLine[2]
			};
			if (partsOfLine[5] == "S")
			{
				donation.Amount = -donation.Amount;
			}
			donation.DonorNo = (uint)donation.Donor.GetHashCode();
			return donation;
		}

		protected override void AppendSecondLine(Donation donation, string[] partsOfLine)
		{
			// just ignore
		}

	}
}

