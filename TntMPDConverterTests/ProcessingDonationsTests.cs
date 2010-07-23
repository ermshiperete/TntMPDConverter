using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TntMPDConverter;

namespace TntMPDConverterTests
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

			internal static Dictionary<int, Dictionary<string, NewDonor>> GetReplacementInfo()
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
			Assert.AreEqual(1, info.Count);
			Assert.IsTrue(info.ContainsKey(999));
			var value = info[999];
			Assert.IsTrue(value.ContainsKey("Markus Mustermann"));
			Assert.AreEqual(997, value["Markus Mustermann"].DonorNo);
			Assert.AreEqual("Mustermann, Markus", value["Markus Mustermann"].Donor);
			Assert.IsTrue(value.ContainsKey("F. Mueller"));
			Assert.AreEqual(996, value["F. Mueller"].DonorNo);
			Assert.AreEqual("Mueller, Franz", value["F. Mueller"].Donor);
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
	}
}
