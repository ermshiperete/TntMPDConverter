// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
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

		protected override int NumberOfFields { get { return 6; }}
		protected override int DonorIndex { get { return 5; }}

		protected override bool IsContinuationLine(string[] partsOfLine)
		{
			return string.IsNullOrEmpty(partsOfLine[1]) || string.IsNullOrEmpty(partsOfLine[2]);
		}

		protected override Donation CreateDonation(string[] partsOfLine, CultureInfo cultureInfo)
		{
			if (!Replacements.IncludeEntry(m_AccountNo, partsOfLine[DonorIndex]))
				return null;

			// 30.10.2012	100,00	H	UM 867	Umb. von Frieder Friederich
			// [1]          [2]    [3]  [4]     [5]
			var donation = new Donation
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
			return donation;
		}
	}
}

