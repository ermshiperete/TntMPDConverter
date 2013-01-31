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
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			MyReplacementManager.Create();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			MyReplacementManager.Finish();
		}

		[Test]
		public void ReplacementFile()
		{
			Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "replace.config"),
				MyReplacementManager.OriginalReplacementFileName);
		}

		[Test]
		public void ReplacementInfo()
		{
			MyReplacementManager.CreateReplacementFile(@"
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
Replace=Friederich, Frieder
[K715]
Include=^.+$");
			var info = MyReplacementManager.Instance.GetReplacementInfo();
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

			// Replacements
			Assert.IsTrue(info.ContainsKey(-1));
			value = info[-1];
			Assert.IsTrue(value.ContainsKey("Berta"));
			Assert.AreEqual("Caesar", value["Berta"].Donor);

			// Regex
			Assert.IsTrue(info.ContainsKey(-2));
			value = info[-2];
			Assert.AreEqual(1, value.Count);
			Assert.IsTrue(value.ContainsKey("^Umb\\..+Friederich$"));
			Assert.AreEqual("Friederich, Frieder", value["^Umb\\..+Friederich$"].Donor);

			// K715
			var include = MyReplacementManager.Instance.GetIncludeInfo();
			Assert.IsTrue(include.ContainsKey(715));
			var includeList = include[715];
			Assert.AreEqual(1, includeList.Count);
			Assert.AreEqual("^.+$", includeList[0]);
		}

		[Test]
		public void AccountSpecificReplacement()
		{
			MyReplacementManager.CreateReplacementFile(@"
# Comment
[999]
Markus Mustermann=997; ""Mustermann, Markus""
F. Mueller=996; Mueller, Franz
Anton=995;Berta");
			var donation = new Donation(80, new DateTime(2010, 06, 01),
				"ungen. ueberw. durch Markus Mustermann", 999);
			MyReplacementManager.Instance.ApplyReplacements(donation);

			AssertEx.DonationEqual(new Donation(80, new DateTime(2010, 06, 01), "Mustermann, Markus", 997),
				donation);
		}

		[Test]
		public void Replacements_AnonDonation()
		{
			MyReplacementManager.CreateReplacementFile(@"
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
			MyReplacementManager.Instance.ApplyReplacements(donation);

			AssertEx.DonationEqual(new Donation(80, new DateTime(2010, 06, 01), "Caesar", 995),
				donation);
		}

		[Test]
		public void Regex_ReplaceEntireString()
		{
			MyReplacementManager.CreateReplacementFile(@"
[Regex]
Pattern=^Umb\..+Friederich$
Replace=Friederich, Frieder");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			MyReplacementManager.Instance.ApplyReplacements(donation);

			Assert.AreEqual("Friederich, Frieder", donation.Donor);
		}

		[Test]
		public void Regex_UseSearchText()
		{
			MyReplacementManager.CreateReplacementFile(@"
[Regex]
Pattern=^.+(Frieder Friederich)$
Replace=$1");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			MyReplacementManager.Instance.ApplyReplacements(donation);

			Assert.AreEqual("Frieder Friederich", donation.Donor);
		}

		[Test]
		public void Regex_ComplexPattern()
		{
			MyReplacementManager.CreateReplacementFile(@"
[Regex]
Pattern=^.+(F[^ ]+) (F[a-z]+)$
Replace=$2, $1");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			MyReplacementManager.Instance.ApplyReplacements(donation);

			Assert.AreEqual("Friederich, Frieder", donation.Donor);
		}

		[Test]
		public void Regex_ReplacePartOfString()
		{
			MyReplacementManager.CreateReplacementFile(@"
[Regex]
Pattern=ie
Replace=ei");
			var donation = new Donation(0, DateTime.Now, "Umb. von Frieder Friederich", 0);

			MyReplacementManager.Instance.ApplyReplacements(donation);

			Assert.AreEqual("Umb. von Freider Freiderich", donation.Donor);
		}

		[Test]
		public void Regex_MultiplePatterns()
		{
			MyReplacementManager.CreateReplacementFile(@"
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

			MyReplacementManager.Instance.ApplyReplacements(donation);

			Assert.AreEqual("Fxeidex Fxeidexich", donation.Donor);
		}

		[Test]
		public void K715_IncludeAll()
		{
			MyReplacementManager.CreateReplacementFile(@"
[K715]
Include=^.+$");
			Assert.IsTrue(MyReplacementManager.Instance.IncludeEntry(
				715, "Umb. von Frieder Friederich"));
		}

		[Test]
		public void K715_IncludeSome()
		{
			MyReplacementManager.CreateReplacementFile(@"
[K715]
Include=Hugo");
			Assert.IsFalse(MyReplacementManager.Instance.IncludeEntry(
				715, "Umb. von Frieder Friederich"));
		}

		[Test]
		public void K715_ExcludeAll()
		{
			MyReplacementManager.CreateReplacementFile("");
			Assert.IsFalse(MyReplacementManager.Instance.IncludeEntry(
				715, "Umb. von Frieder Friederich"));
		}
	}
}

