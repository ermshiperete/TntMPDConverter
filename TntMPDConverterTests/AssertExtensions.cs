// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using NUnit.Framework;

namespace TntMPDConverter
{
	public static class AssertEx
	{
		public static void DonationEqual(Donation expected, Donation actual)
		{
			if (expected.DonorNo != UInt32.MaxValue)
				Assert.AreEqual(expected.DonorNo, actual.DonorNo);
			Assert.AreEqual(expected.Donor, actual.Donor);
			Assert.AreEqual(expected.Date, actual.Date);
			Assert.AreEqual(expected.Amount, actual.Amount);
			Assert.AreEqual(expected.Remarks, actual.Remarks);
			Assert.AreEqual(expected.TenderedAmount, actual.TenderedAmount);
			Assert.AreEqual(expected.TenderedCurrency, actual.TenderedCurrency);
		}

	}
}

