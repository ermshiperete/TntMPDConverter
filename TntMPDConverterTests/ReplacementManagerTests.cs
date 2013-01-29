using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class ReplacementManagerTests
	{
		private class MyReplacementManager: ReplacementManager
		{
			public string OriginalReplacementFileName { get; private set;}
			public MyReplacementManager(string fileName)
			{
				OriginalReplacementFileName = ReplacementFileName;
				ReplacementFileName = fileName;
			}

			public string ReplacementFileNameForTests
			{
				get { return ReplacementFileName;}
				set { ReplacementFileName = value; }
			}

			public Dictionary<int, Dictionary<string, Replacement>> GetReplacementInfo()
			{
				return ReplacementInfo;
			}

			public void ReReadReplacementFile()
			{
				UpdateReplacementInfo();
			}
		}

		private MyReplacementManager ReplacementMgr;

		private void CreateReplacementFile(string content)
		{
			using (var writer = new StreamWriter(ReplacementMgr.ReplacementFileNameForTests))
			{
				writer.WriteLine(content);
			}
			ReplacementMgr.ReReadReplacementFile();
		}

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			ReplacementMgr = new MyReplacementManager(Path.GetTempFileName());
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			File.Delete(ReplacementMgr.ReplacementFileNameForTests);
		}

		[Test]
		public void ReplacementFile()
		{
			Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "replace.config"),
				ReplacementMgr.OriginalReplacementFileName);
		}

		[Test]
		public void ReplacementInfo()
		{
			CreateReplacementFile(@"
# Comment
[999]
Markus Mustermann=997; ""Mustermann, Markus""
F. Mueller=996; Mueller, Franz
Anton=995;Berta
[Replacements]
Umb. von Frieder Friederich=Friederich, Frieder
Berta=Caesar
[Regex]
Pattern=^Umb\..+Friederich$
Replace=Friederich, Frieder");

			var info = ReplacementMgr.GetReplacementInfo();
			Assert.AreEqual(3, info.Count);
			Assert.IsTrue(info.ContainsKey(999));
			var value = info[999];
			Assert.IsTrue(value.ContainsKey("Markus Mustermann"));
			Assert.IsInstanceOf<ReplacementManager.NewDonor>(value["Markus Mustermann"]);
			var newDonor = value["Markus Mustermann"] as ReplacementManager.NewDonor;
			Assert.AreEqual(997, newDonor.DonorNo);
			Assert.AreEqual("Mustermann, Markus", newDonor.Donor);
			Assert.IsTrue(value.ContainsKey("F. Mueller"));
			Assert.IsInstanceOf<ReplacementManager.NewDonor>(value["F. Mueller"]);
			newDonor = value["F. Mueller"] as ReplacementManager.NewDonor;
			Assert.AreEqual(996, newDonor.DonorNo);
			Assert.AreEqual("Mueller, Franz", newDonor.Donor);
			Assert.IsTrue(info.ContainsKey(-1));
			value = info[-1];
			Assert.IsTrue(value.ContainsKey("Berta"));
			Assert.AreEqual("Caesar", value["Berta"].Donor);
			Assert.IsTrue(info.ContainsKey(-2));
			value = info[-2];
			Assert.AreEqual(1, value.Count);
			Assert.IsTrue(value.ContainsKey("^Umb\\..+Friederich$"));
			Assert.AreEqual("Friederich, Frieder", value["^Umb\\..+Friederich$"].Donor);
		}

		[Test]
		public void AccountSpecificReplacement()
		{
			CreateReplacementFile(@"
# Comment
[999]
Markus Mustermann=997; ""Mustermann, Markus""
F. Mueller=996; Mueller, Franz
Anton=995;Berta");

			var donation = new Donation(80, new DateTime(2010, 06, 01),
				"ungen. ueberw. durch Markus Mustermann", 999);
			ReplacementMgr.ApplyReplacements(donation);

			AssertEx.DonationEqual(new Donation(80, new DateTime(2010, 06, 01), "Mustermann, Markus", 997),
				donation);
		}

		[Test]
		public void Replacements_AnonDonation()
		{
			CreateReplacementFile(@"
# Comment
[999]
Markus Mustermann=997; ""Mustermann, Markus""
F. Mueller=996; Mueller, Franz
Anton=995;Berta
[Replacements]
Umb. von Frieder Friederich=Friederich, Frieder
Berta=Caesar");

			var donation = new Donation(80, new DateTime(2010, 06, 01),
				"ungen. ueberw. durch Anton", 999);
			ReplacementMgr.ApplyReplacements(donation);

			AssertEx.DonationEqual(new Donation(80, new DateTime(2010, 06, 01), "Caesar", 995),
				donation);
		}

		[Test]
		public void Regex_ReplaceEntireString()
		{
			CreateReplacementFile(@"
[Regex]
Pattern=^Umb\..+Friederich$
Replace=Friederich, Frieder");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			ReplacementMgr.ApplyReplacements(donation);

			Assert.AreEqual("Friederich, Frieder", donation.Donor);
		}

		[Test]
		public void Regex_UseSearchText()
		{
			CreateReplacementFile(@"
[Regex]
Pattern=^.+(Frieder Friederich)$
Replace=$1");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			ReplacementMgr.ApplyReplacements(donation);

			Assert.AreEqual("Frieder Friederich", donation.Donor);
		}

		[Test]
		public void Regex_ComplexPattern()
		{
			CreateReplacementFile(@"
[Regex]
Pattern=^.+(F[^ ]+) (F[a-z]+)$
Replace=$2, $1");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			ReplacementMgr.ApplyReplacements(donation);

			Assert.AreEqual("Friederich, Frieder", donation.Donor);
		}

		[Test]
		public void Regex_ReplacePartOfString()
		{
			CreateReplacementFile(@"
[Regex]
Pattern=ie
Replace=ei");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			ReplacementMgr.ApplyReplacements(donation);

			Assert.AreEqual("Umb. von Freider Freiderich", donation.Donor);
		}

		[Test]
		public void Regex_MultiplePatterns()
		{
			CreateReplacementFile(@"
[Regex]
Pattern=ie
Replace=ei
Pattern=""Umb\. von ""
Replace=
Pattern=Frieder
Replace=Hugo
Pattern=r
Replace=x");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			ReplacementMgr.ApplyReplacements(donation);

			Assert.AreEqual("Fxeidex Fxeidexich", donation.Donor);
		}
	}
}

