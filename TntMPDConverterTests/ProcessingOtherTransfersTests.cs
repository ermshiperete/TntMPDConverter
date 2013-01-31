using System;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class ProcessingOtherTransfersTests
	{
		[Test]
		public void Transfer()
		{
			var reader = new FakeScanner(@"
	23.11.2012	US$	209,64	162,20	H	8021	SWZ Member gift");
			var processingDonations = new ProcessingOtherTransfers(reader);
			AssertEx.DonationEqual(new Donation(162.20m, new DateTime(2012, 11, 23),
				"SWZ Member gift", UInt32.MaxValue, "Netto; US$ 209,64"), processingDonations.NextDonation);
		}

		[Test]
		public void MultipleMonthsGiveSameDonorNo()
		{
			var reader = new FakeScanner(@"
	23.11.2012	US$	209,64	162,20	H	8021	SWZ Member gift");
			var processingDonations = new ProcessingOtherTransfers(reader);
			var donation = processingDonations.NextDonation;
			var firstDonorNo = donation.DonorNo;

			reader = new FakeScanner(@"
	21.12.2012	US$	450,00	423,18	H	8022	SWZ Member gift");
			processingDonations = new ProcessingOtherTransfers(reader);
			donation = processingDonations.NextDonation;
			Assert.AreEqual(firstDonorNo, donation.DonorNo);
		}
	}
}

