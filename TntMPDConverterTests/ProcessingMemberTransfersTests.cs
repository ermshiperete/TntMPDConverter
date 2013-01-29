using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TntMPDConverter;

namespace TntMPDConverter
{
	[TestFixture]
	public class ProcessingMemberTransfersTests
	{
		[Test]
		public void Transfer()
		{
			var reader = new FakeScanner(@"
	30.10.2012	100,00	H	UM 867	Frieder Friederich");
			var processingDonations = new ProcessingMemberTransfers(reader);
			AssertEx.DonationEqual(new Donation(100.00m, new DateTime(2012, 10, 30), "Frieder Friederich", 867),
				processingDonations.NextDonation);
		}
	}
}

