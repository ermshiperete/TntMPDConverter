// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class ProcessingOtherTransfersTests
	{
		[TestFixtureSetUp]
		public virtual void FixtureSetUp()
		{
			MyReplacementManager.Create();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			MyReplacementManager.Finish();
		}

		[Test]
		public void Transfer()
		{
			MyReplacementManager.CreateReplacementFile(string.Empty);
			var reader = new FakeScanner(@"
	23.11.2012	US$	209,64	162,20	H	8021	SWZ Member gift");
			var processingDonations = new ProcessingOtherTransfers(3224, reader);
			AssertEx.DonationEqual(new Donation(162.20m, new DateTime(2012, 11, 23),
				"SWZ Member gift", UInt32.MaxValue, "Netto; US$ 209,64", 209.64m, "USD"), processingDonations.NextDonation);
		}

		[Test]
		public void Excluded()
		{
			MyReplacementManager.CreateReplacementFile(@"
[K3224]
Exclude=^.+$");
			var reader = new FakeScanner(@"
	23.11.2012	US$	209,64	162,20	H	8021	SWZ Member gift");
			var processingDonations = new ProcessingOtherTransfers(3224, reader);
			Assert.That(processingDonations.NextDonation, Is.Null);
		}

		[Test]
		public void MultipleMonthsGiveSameDonorNo()
		{
			MyReplacementManager.CreateReplacementFile(string.Empty);
			var reader = new FakeScanner(@"
	23.11.2012	US$	209,64	162,20	H	8021	SWZ Member gift");
			var processingDonations = new ProcessingOtherTransfers(3224, reader);
			var donation = processingDonations.NextDonation;
			var firstDonorNo = donation.DonorNo;

			reader = new FakeScanner(@"
	21.12.2012	US$	450,00	423,18	H	8022	SWZ Member gift");
			processingDonations = new ProcessingOtherTransfers(3224, reader);
			donation = processingDonations.NextDonation;
			Assert.AreEqual(firstDonorNo, donation.DonorNo);
		}

		[Test]
		public void MultiLineTransfer()
		{
			MyReplacementManager.CreateReplacementFile(string.Empty);
			var reader = new FakeScanner(@"
	23.11.2012	US$	209,64	162,20	H	8021	437921 BT-BANK TRANSFER
							OPP_TransAmount=-209,64");
			var processingDonations = new ProcessingOtherTransfers(3224, reader);
			AssertEx.DonationEqual(new Donation(162.20m, new DateTime(2012, 11, 23),
				"437921 BT-BANK TRANSFER", UInt32.MaxValue, "Netto; US$ 209,64", 209.64m, "USD"),
				processingDonations.NextDonation);
		}

	}
}

