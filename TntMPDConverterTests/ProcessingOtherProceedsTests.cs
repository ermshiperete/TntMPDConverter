// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class ProcessingOtherProceedsTests
	{
		[Test]
		public void MultiLine()
		{
			var reader = new FakeScanner(@"
	01.06.2010	80,00	H	KD	Mustermann, Markus
					Continued");
			var donation = new ProcessingOtherProceeds(reader).NextDonation;
			AssertEx.DonationEqual(new Donation(80, new DateTime(2010, 06, 01),
				"Mustermann, Markus Continued", 998), donation);
		}
	}
}

