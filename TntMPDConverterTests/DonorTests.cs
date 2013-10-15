// Copyright (c) 2011, SIL International. All Rights Reserved.
// <copyright from='2011' to='2011' company='SIL International'>
// 	Copyright (c) 2011, SIL International. All Rights Reserved.
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
using System;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class DonorTests
	{
		[Test]
		public void Names_FirstLast()
		{
			var donor = new Donor(1, "Mustermann, Markus", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Mustermann", donor.LastName);
			Assert.AreEqual("Markus", donor.FirstName);
			Assert.AreEqual("P", donor.PersonType);
		}

		[Test]
		public void Names_Organization()
		{
			var donor = new Donor(1, "Organization", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Organization", donor.LastName);
			Assert.AreEqual(string.Empty, donor.FirstName);
			Assert.AreEqual("O", donor.PersonType);
		}

		[Test]
		public void Title()
		{
			var donor = new Donor(1, "Mustermann, Dr. Markus", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Mustermann", donor.LastName);
			Assert.AreEqual("Markus", donor.FirstName);
			Assert.AreEqual("P", donor.PersonType);
			Assert.AreEqual("Dr.", donor.Title);
		}

		[Test]
		public void Spouse()
		{
			var donor = new Donor(1, "Mustermann, Markus und Martina", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Mustermann", donor.LastName);
			Assert.AreEqual("Markus", donor.FirstName);
			Assert.AreEqual("", donor.SpouseLastName);
			Assert.AreEqual("Martina", donor.SpouseFirstName);
			Assert.AreEqual("P", donor.PersonType);
			Assert.AreEqual("", donor.Title);
		}

		[Test]
		public void SpouseAndTitle()
		{
			var donor = new Donor(1, "Mustermann, Prof. Dr. Dr. h.c. Markus & Pfarrerin Martina", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Mustermann", donor.LastName);
			Assert.AreEqual("Markus", donor.FirstName);
			Assert.AreEqual("Prof. Dr. Dr. h.c.", donor.Title);
			Assert.AreEqual("", donor.SpouseLastName);
			Assert.AreEqual("Martina", donor.SpouseFirstName);
			Assert.AreEqual("P", donor.PersonType);
			Assert.AreEqual("Pfarrerin", donor.SpouseTitle);
		}

		[Test]
		public void Spouse_UDot()
		{
			var donor = new Donor(1, "Mustermann, Markus u. Martina", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Mustermann", donor.LastName);
			Assert.AreEqual("Markus", donor.FirstName);
			Assert.AreEqual("", donor.SpouseLastName);
			Assert.AreEqual("Martina", donor.SpouseFirstName);
			Assert.AreEqual("P", donor.PersonType);
			Assert.AreEqual("", donor.Title);
		}

		[Test]
		public void Spouse_Title()
		{
			var donor = new Donor(1, "Mustermann, Markus und Dr. Martina", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Mustermann", donor.LastName);
			Assert.AreEqual("Markus", donor.FirstName);
			Assert.AreEqual("", donor.SpouseLastName);
			Assert.AreEqual("Martina", donor.SpouseFirstName);
			Assert.AreEqual("P", donor.PersonType);
			Assert.AreEqual("", donor.Title);
			Assert.AreEqual("Dr.", donor.SpouseTitle);
		}

		[Test]
		public void Spouse_DifferentLastName()
		{
			var donor = new Donor(1, "Mustermann, Markus und Musterfrau, Martina", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Mustermann", donor.LastName);
			Assert.AreEqual("Markus", donor.FirstName);
			Assert.AreEqual("Musterfrau", donor.SpouseLastName);
			Assert.AreEqual("Martina", donor.SpouseFirstName);
			Assert.AreEqual("P", donor.PersonType);
			Assert.AreEqual("", donor.Title);
			Assert.AreEqual("", donor.SpouseTitle);
		}

		[Test]
		public void Spouse_DifferentLastNameTitle()
		{
			var donor = new Donor(1, "Mustermann, Markus und Musterfrau, Dr. Martina", string.Empty, string.Empty, string.Empty,
					null, string.Empty, 0, 0);
			Assert.AreEqual("Mustermann", donor.LastName);
			Assert.AreEqual("Markus", donor.FirstName);
			Assert.AreEqual("Musterfrau", donor.SpouseLastName);
			Assert.AreEqual("Martina", donor.SpouseFirstName);
			Assert.AreEqual("P", donor.PersonType);
			Assert.AreEqual("", donor.Title);
			Assert.AreEqual("Dr.", donor.SpouseTitle);
		}

	}
}
