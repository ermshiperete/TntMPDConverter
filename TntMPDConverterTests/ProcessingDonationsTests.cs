// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using NUnit.Framework;
using TntMPDConverter;

namespace TntMPDConverter
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class ProcessingDonationsTests
	{
		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting a normal donation
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void NormalDonation()
		{
			var reader = new FakeScanner(@"
	16805	01.06.2010	80,00	H	KD	Mustermann, Markus");
			var donation = new ProcessingDonations(reader).NextDonation;
			AssertEx.DonationEqual(new Donation(80, new DateTime(2010, 06, 01), "Mustermann, Markus", 16805),
				donation);
		}

		[Test]
		public void SkipPageBreak()
		{
			var reader = new FakeScanner(@"
	16448	22.06.2010	51,13	H	KD 	Mustermann, Markus
	21860	25.06.2010	80,00	H	KD 	Mueller, Frieda

Projekt	12345  Missionar, Fritz	Soll €	Haben €
	16800	28.06.2010	26,00	H	KD 	doppelt
	11706	30.06.2010	16,00	H	KD 	Musterfrau, Elfriede");
			var processingDonations = new ProcessingDonations(reader);
			AssertEx.DonationEqual(new Donation(51.13m, new DateTime(2010, 06, 22), "Mustermann, Markus", 16448),
				processingDonations.NextDonation);
			AssertEx.DonationEqual(new Donation(80.00m, new DateTime(2010, 06, 25), "Mueller, Frieda", 21860),
				processingDonations.NextDonation);
			AssertEx.DonationEqual(new Donation(26.00m, new DateTime(2010, 06, 28), "doppelt", 16800),
				processingDonations.NextDonation);
			AssertEx.DonationEqual(new Donation(16.00m, new DateTime(2010, 06, 30), "Musterfrau, Elfriede", 11706),
				processingDonations.NextDonation);
		}

		[Test]
		public void MultiLine()
		{
			var reader = new FakeScanner(@"
	16805	01.06.2010	80,00	H	KD	Mustermann, Markus
						Continued");
			var donation = new ProcessingDonations(reader).NextDonation;
			AssertEx.DonationEqual(new Donation(80, new DateTime(2010, 06, 01),
				"Mustermann, Markus Continued", 16805), donation);
		}
	}
}
