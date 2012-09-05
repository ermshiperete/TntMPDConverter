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
			Assert.AreEqual(expected.Plz, actual.Plz, "Wrong zip code");
			Assert.AreEqual(expected.City, actual.City, "Wrong donor city");
			CollectionAssert.AreEqual(expected.PhoneNos, actual.PhoneNos, "Wrong phone numbers");
			Assert.AreEqual(expected.Email, actual.Email, "Wrong email");
			Assert.AreEqual(expected.DonationsCount, actual.DonationsCount, "Wrong number of donations");
			Assert.AreEqual(expected.Amount, actual.Amount, "Wrong donation amount");
		}

		[Test]
		public void DonorNoPhone()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	1	100,00	51,28
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new string[] { }, string.Empty, 1, 100.00m),
				donorProcessor.NextDonor);
		}

		[Test]
		public void DonorNoPhoneEurOnly()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	1	100,00
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new string[] { }, string.Empty, 1, 100.00m),
				donorProcessor.NextDonor);
		}

		[Test]
		public void DonorLargeAmount()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	1	2.100,00	1.051,28
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new string[] { }, string.Empty, 1, 2100.00m),
				donorProcessor.NextDonor);
		}

		[Test]
		public void DonorWithPhone()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	p: 02736/1234561	1	100,00	51,28
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new[] {"p: 02736/1234561"}, string.Empty, 1, 100.00m), donorProcessor.NextDonor);
		}

		[Test]
		public void DonorWithPhoneEurOnly()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	p: 02736/1234561	1	100,00
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new[] {"p: 02736/1234561"}, string.Empty, 1, 100.00m), donorProcessor.NextDonor);
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
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new[] {"p: 02736/1234561, d: 01234/5678"}, string.Empty, 1, 100.00m), donorProcessor.NextDonor);
		}

		/// <summary>
		/// Contains second phone number and email address
		/// </summary>
		[Test]
		public void DonorSecondPhoneAndEmail()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	p: 02736/1234561	1	100,00	51,28
	77123 Irgendwo	02736-9876
	markus@example.com");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new[] { "p: 02736/1234561", "02736-9876"}, "markus@example.com", 1, 100.00m), donorProcessor.NextDonor);
		}

		/// <summary>
		/// Contains second phone number
		/// </summary>
		[Test]
		public void DonorSecondPhone()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	p: 02736/1234561	1	100,00	51,28
	77123 Irgendwo	02736-9876");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new[] { "p: 02736/1234561", "02736-9876" }, string.Empty, 1, 100.00m), donorProcessor.NextDonor);
		}

		/// <summary>
		/// Contains phone number and email address
		/// </summary>
		[Test]
		public void DonorPhoneAndEmail()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	p: 02736/1234561	1	100,00	51,28
	77123 Irgendwo
	markus@example.com");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new[] { "p: 02736/1234561" }, "markus@example.com", 1, 100.00m), donorProcessor.NextDonor);
		}

		/// <summary>
		/// Contains phone number that stretches over two lines and email address
		/// </summary>
		[Test]
		public void DonorLongPhoneAndEmail()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	p: 02736/1234561, d: 012	1	100,00	51,28
	34/5678
	77123 Irgendwo
	markus@example.com");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new[] { "p: 02736/1234561, d: 01234/5678" }, "markus@example.com", 1, 100.00m), donorProcessor.NextDonor);
		}

		/// <summary>
		/// Contains only email address
		/// </summary>
		[Test]
		public void DonorEmail()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	1	100,00	51,28
	77123 Irgendwo
	markus@example.com");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14", "77123", "Irgendwo",
					new string[] {  }, "markus@example.com", 1, 100.00m), donorProcessor.NextDonor);
		}

		/// <summary>
		/// Tests that we skip to the start of addresses
		/// </summary>
		[Test]
		public void SkipToDonor()
		{
			var reader = new FakeScanner(@"Spenderübersicht
Spender-	Telefon (privat, dienstl.)	Spenden
Nr.	Name	Adresse	Fax, E-Mail	Anz.	€	€*
	16805	Mustermann, Markus	Schlossallee 14	1	100,00	51,28
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14",
					"77123", "Irgendwo", new string[] {  }, string.Empty, 1, 100.00m),
				donorProcessor.NextDonor);
		}

		/// <summary>
		/// Tests that we skip a page break in the middle of an addresses
		/// </summary>
		[Test]
		public void SkipPageBreak()
		{
			var reader = new FakeScanner(@"	16805	Mustermann, Markus	Schlossallee 14	1	100,00	51,28

Projekt:	12345	Markus Missionar	Soll €	Haben €
	77123 Irgendwo");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Mustermann, Markus", "Schlossallee 14",
					"77123", "Irgendwo", new string[] {  }, string.Empty, 1, 100.00m),
				donorProcessor.NextDonor);
		}

		/// <summary>
		/// Contains multiline organization name
		/// </summary>
		[Test]
		[Ignore("Issue #4")]
		public void DonorMultilineOrganization()
		{
			var reader = new FakeScanner(@"	16805	Organization	Schlossallee 14	1	100,00	51,28
	Irgendwo
	77123 Irgendwo
	markus@example.com");

			var donorProcessor = new ProcessDonors(reader);
			AssertDonorEqual(new Donor(16805, "Organization Irgendwo", "Schlossallee 14", "77123", "Irgendwo",
					new string[] {  }, "markus@example.com", 1, 100.00m), donorProcessor.NextDonor);
		}

	}
}

