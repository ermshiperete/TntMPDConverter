// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class DonationTests
	{
		[SetUp]
		public void SetUp()
		{
			Donation.Reset();
		}

		[Test]
		public void BookingIdFromDate()
		{
			var donation = new Donation(10, new DateTime(2011, 01, 01), "Markus Mustermann", 4711);
			Assert.AreEqual("20110101047111", donation.BookingId);
		}

		[Test]
		public void TwoDonationsOnSameDate()
		{
			var donation = new Donation(10, new DateTime(2011, 01, 01), "Markus Mustermann", 4711);
			var secondDonation = new Donation(100, new DateTime(2011, 01, 01), "Markus Mustermann", 4711);
			Assert.AreEqual("20110101047111", donation.BookingId);
			Assert.AreEqual("20110101047112", secondDonation.BookingId);
		}

		[Test]
		public void ReplacingDonor()
		{
			var donation = new Donation(10, new DateTime(2011, 01, 01), "Markus Mustermann", 4711);
			donation.Donor = "ABC";
			donation.DonorNo = 12345;
			Assert.AreEqual("20110101123451", donation.BookingId);
		}
	}
}

