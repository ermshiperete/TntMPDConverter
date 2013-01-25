using System;
using System.Collections.Generic;
using System.IO;
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
		#region class MyProcessingDontations
		private class MyProcessingDonations : ProcessingDonations
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
		public void ReplacementFile()
		{
			Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "replace.config"),
				MyProcessingDonations.GetReplacementFileName());
		}

		[Test]
		public void ReplacementInfo()
		{
			MyProcessingDonations.SetReplacementFileName(ReplacementFileName);
			var info = MyProcessingDonations.GetReplacementInfo();
			Assert.AreEqual(2, info.Count);
			Assert.IsTrue(info.ContainsKey(999));
			var value = info[999];
			Assert.IsTrue(value.ContainsKey("Markus Mustermann"));
			Assert.IsInstanceOf<ProcessingDonations.NewDonor>(value["Markus Mustermann"]);
			var newDonor = value["Markus Mustermann"] as ProcessingDonations.NewDonor;
			Assert.AreEqual(997, newDonor.DonorNo);
			Assert.AreEqual("Mustermann, Markus", newDonor.Donor);
			Assert.IsTrue(value.ContainsKey("F. Mueller"));
			Assert.IsInstanceOf<ProcessingDonations.NewDonor>(value["F. Mueller"]);
			newDonor = value["F. Mueller"] as ProcessingDonations.NewDonor;
			Assert.AreEqual(996, newDonor.DonorNo);
			Assert.AreEqual("Mueller, Franz", newDonor.Donor);
			value = info[-1];
			Assert.IsTrue(value.ContainsKey("Berta"));
			Assert.AreEqual("Caesar", value["Berta"].Donor);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting a normal donation
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void NormalDonation()
		{
			var reader = new Scanner(null);
			reader.UnreadLine("\t16805\t01.06.2010\t80,00\tH\tKD\tMustermann, Markus");
			var donation = new ProcessingDonations(reader).NextDonation;
			AssertDonationEqual(new Donation(80, new DateTime(2010, 06, 01), "Mustermann, Markus", 16805), 
				donation);
		}

		[Test]
		public void ReplaceAnonDonation()
		{
			var reader = new Scanner(null);
			reader.UnreadLine("\t999\t01.06.2010\t80,00\tH\tKD\tungen. ueberw. durch Markus Mustermann");
			var processingDonations = new MyProcessingDonations(reader);
			MyProcessingDonations.SetReplacementFileName(ReplacementFileName);
			var donation = processingDonations.NextDonation;
			AssertDonationEqual(new Donation(80, new DateTime(2010, 06, 01), "Mustermann, Markus", 997),
				donation);
		}

		[Test]
		public void DoubleReplaceAnonDonation()
		{
			var reader = new Scanner(null);
			reader.UnreadLine("\t999\t01.06.2010\t80,00\tH\tKD\tungen. ueberw. durch Anton");
			var processingDonations = new MyProcessingDonations(reader);
			MyProcessingDonations.SetReplacementFileName(ReplacementFileName);
			var donation = processingDonations.NextDonation;
			AssertDonationEqual(new Donation(80, new DateTime(2010, 06, 01), "Caesar", 995),
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
			var processingDonations = new MyProcessingDonations(reader);
			MyProcessingDonations.SetReplacementFileName(ReplacementFileName);
			AssertDonationEqual(new Donation(51.13m, new DateTime(2010, 06, 22), "Mustermann, Markus", 16448),
				processingDonations.NextDonation);
			AssertDonationEqual(new Donation(80.00m, new DateTime(2010, 06, 25), "Mueller, Frieda", 21860),
				processingDonations.NextDonation);
			AssertDonationEqual(new Donation(26.00m, new DateTime(2010, 06, 28), "doppelt", 16800),
				processingDonations.NextDonation);
			AssertDonationEqual(new Donation(16.00m, new DateTime(2010, 06, 30), "Musterfrau, Elfriede", 11706),
				processingDonations.NextDonation);
		}
	}
}
