// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System;
using NUnit.Framework;
using TntMPDConverter;

namespace TntMPDConverter
{
	[TestFixture]
	public class ProcessingMemberTransfersTests
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
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
			MyReplacementManager.CreateReplacementFile(@"
[K715]
Include=^.+$");
			var reader = new FakeScanner(@"
	30.10.2012	100,00	H	UM 867	Frieder Friederich");
			var processingDonations = new ProcessingMemberTransfers(715, reader);
			AssertEx.DonationEqual(new Donation(100.00m, new DateTime(2012, 10, 30),
				"Frieder Friederich", UInt32.MaxValue),
				processingDonations.NextDonation);
		}

		[Test]
		public void NotIncluded()
		{
			MyReplacementManager.CreateReplacementFile(@"
[K715]
Include=Something");
			var reader = new FakeScanner(@"
	30.10.2012	100,00	H	UM 867	Frieder Friederich");
			var processingDonations = new ProcessingMemberTransfers(715, reader);
			Assert.That(processingDonations.NextDonation, Is.Null);
		}

		[Test]
		public void MultiLine()
		{
			MyReplacementManager.CreateReplacementFile(@"
[K715]
Include=^.+$");
			var reader = new FakeScanner(@"
	01.06.2010	80,00	H	UM 867	Mustermann, Markus
					Continued");
			var donation = new ProcessingMemberTransfers(715, reader).NextDonation;
			AssertEx.DonationEqual(new Donation(80, new DateTime(2010, 06, 01),
				"Mustermann, Markus Continued", UInt32.MaxValue), donation);
		}
	}
}

