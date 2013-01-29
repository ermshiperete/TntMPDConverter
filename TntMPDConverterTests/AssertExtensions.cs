using System;
using NUnit.Framework;

namespace TntMPDConverter
{
	public static class AssertEx
	{
		public static void DonationEqual(Donation expected, Donation actual)
		{
			Assert.AreEqual(expected.DonorNo, actual.DonorNo);
			Assert.AreEqual(expected.Donor, actual.Donor);
			Assert.AreEqual(expected.Date, actual.Date);
			Assert.AreEqual(expected.Amount, actual.Amount);
		}

	}
}

