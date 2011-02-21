// Copyright (c) 2011, Eberhard Beilharz. All Rights Reserved.
using System;
using NUnit.Framework;
namespace TntMPDConverter
{
	[TestFixture]
	public class ProcessDonorsTests
	{
		private static void AssertDonorEqual(Donor expected, Donor actual)
		{
			Assert.AreEqual(expected.DonorNo, actual.DonorNo, "Wrong donor number");
			Assert.AreEqual(expected.Name, actual.Name, "Wrong donor name");
			Assert.AreEqual(expected.Street, actual.Street, "Wrong donor street");
			Assert.AreEqual(expected.City, actual.City, "Wrong donor city");
			Assert.AreEqual(expected.PhoneNos, actual.PhoneNos, "Wrong phone numbers");
			Assert.AreEqual(expected.DonationsCount, actual.DonationsCount, "Wrong number of donations");
			Assert.AreEqual(expected.Amount, actual.Amount, "Wrong donation amount");
		}

		[Test]
		public void DonorNoPhone()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	1	100,00	51,28
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123 Irgendwo",
					string.Empty, 1, 51.28m),
				donorProcessor.NextDonor);
		}

		[Test]
		public void DonorWithPhone()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	p: 02736/1234561	1	100,00	51,28
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123 Irgendwo",
					"p: 02736/1234561", 1, 51.28m), donorProcessor.NextDonor);
		}

		/// <summary>
		/// Phone number is to long to fit in one line, continues in second line
		/// </summary>
		[Test]
		public void DonorWithMultilinePhone()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	p: 02736/1234561, d: 012	1	100,00	51,28
	34/5678
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123 Irgendwo",
					"p: 02736/1234561, d: 01234/5678", 1, 51.28m), donorProcessor.NextDonor);
		}

// second phone no + email
//	14559	Beilharz, Heiner	Hochberg 22	p: 07403 914939	1	310,00	158,97
//	78664 Eschbronn	07403-914939
//	H.Beilharz@gmx.net

// phone no + email
//	4078	Beilharz, Christine	Buchenstr. 7	p: 07032/330843	1	300,00	153,85
//	71126 Gäufelden
//	c.beilharz@gmx.net

// continued phone no + email
//	15800	Bourwieg, Karsten	Cäsariusstr. 6	p: 0228-9090790  d: +49 (0)228 	1	80,00	41,03
//	14-5760
//	53173 Bonn
//	bourwieg@hotmail.com

// no phone, email
// second phone
	}
}

