using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TntMPDConverter;

namespace TntMPDConverter
{
	[TestFixture]
	public class ProcessingMemberTransfersTests
	{
		#region class MyProcessingDontations
		private class MyProcessingDonations : ProcessingMemberTransfers
		{
			public MyProcessingDonations(Scanner reader)
				: base(reader)
			{
				OriginalReplacementFileName = ReplacementFileName;
			}

			private static string OriginalReplacementFileName { get; set; }

			internal static Dictionary<int, Dictionary<string, Replacement>> GetReplacementInfo()
			{
				return ReplacementInfo;
			}

			internal static string GetReplacementFileName()
			{
				if (OriginalReplacementFileName == null)
					return ReplacementFileName;
				return OriginalReplacementFileName;
			}

			internal static void SetReplacementFileName(string fileName)
			{
				ReplacementFileName = fileName;
				UpdateReplacementInfo();
			}
		}
		#endregion

		private string ReplacementFileName { get; set; }

		private static void AssertDonationEqual(Donation expected, Donation actual)
		{
			Assert.AreEqual(expected.DonorNo, actual.DonorNo);
			Assert.AreEqual(expected.Donor, actual.Donor);
			Assert.AreEqual(expected.Date, actual.Date);
			Assert.AreEqual(expected.Amount, actual.Amount);
		}

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			ReplacementFileName = Path.GetTempFileName();
			using (var writer = new StreamWriter(ReplacementFileName))
			{
				writer.WriteLine("# Comment");
				writer.WriteLine("[999]");
				writer.WriteLine("Markus Mustermann=997; \"Mustermann, Markus\"");
				writer.WriteLine("F. Mueller=996; Mueller, Franz");
				writer.WriteLine("Anton=995;Berta");
				writer.WriteLine("[Replacements]");
				writer.WriteLine("Umb. von Frieder Friederich=Friederich, Frieder");
				writer.WriteLine("Berta=Caesar");
			}
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			File.Delete(ReplacementFileName);
		}

		[TearDown]
		public void TearDown()
		{
			MyProcessingDonations.SetReplacementFileName(MyProcessingDonations.GetReplacementFileName());
		}

		[Test]
		public void Transfer()
		{
			var reader = new FakeScanner(@"
	30.10.2012	100,00	H	UM 867	Umb. von Frieder Friederich");
			var processingDonations = new MyProcessingDonations(reader);
			MyProcessingDonations.SetReplacementFileName(ReplacementFileName);
			AssertDonationEqual(new Donation(100.00m, new DateTime(2012, 10, 30), "Friederich, Frieder", 867),
				processingDonations.NextDonation);
		}
	}
}

