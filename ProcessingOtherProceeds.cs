// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Globalization;

namespace TntMPDConverter
{
	public class ProcessingOtherProceeds : ProcessingDonations
	{
		public ProcessingOtherProceeds(Scanner reader) : base(reader)
		{
		}

		protected override int NumberOfFields { get { return 6; }}
		protected override int DonorIndex { get { return 5; }}

		protected override bool IsContinuationLine(string[] partsOfLine)
		{
			return string.IsNullOrEmpty(partsOfLine[1]) || string.IsNullOrEmpty(partsOfLine[2]);
		}

		protected override Donation CreateDonation(string[] partsOfLine, CultureInfo cultureInfo)
		{
			var donation = new Donation
			{
				DonorNo = 998,
				Date = Convert.ToDateTime(partsOfLine[1], cultureInfo),
				Amount = Convert.ToDecimal(partsOfLine[2], cultureInfo)
			};
			if (partsOfLine[3] == "S")
			{
				donation.Amount = -donation.Amount;
			}
			donation.Donor = partsOfLine[5];
			return donation;
		}
	}
}

