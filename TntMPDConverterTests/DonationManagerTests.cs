using System;
using NUnit.Framework;
namespace TntMPDConverter
{
	[TestFixture]
	public class DonationManagerTests
	{
		private DonationManager m_Manager;

		[SetUp]
		public void SetUp()
		{
			m_Manager = new DonationManager();
		}

		[Test]
		public void AddDonation()
		{
			var donation = new Donation(10, new DateTime(2010, 11, 12), "Mustermann", 12345);
			Assert.AreEqual(1, m_Manager.GetBookingId(donation));
		}

		[Test]
		public void AddSecondDonationOnSameDay()
		{
			m_Manager.GetBookingId(new Donation(10, new DateTime(2010, 11, 12), "Mustermann", 12345));
			Assert.AreEqual(2, m_Manager.GetBookingId(new Donation(100, new DateTime(2010, 11, 12), "Mustermann", 12345)));
		}

		[Test]
		public void AddIdenticalDonationOnSameDay()
		{
			m_Manager.GetBookingId(new Donation(10, new DateTime(2010, 11, 12), "Mustermann", 12345));
			Assert.AreEqual(2, m_Manager.GetBookingId(new Donation(10, new DateTime(2010, 11, 12), "Mustermann", 12345)));
		}

		[Test]
		public void AddDifferentDonationOnSameDay()
		{
			m_Manager.GetBookingId(new Donation(10, new DateTime(2010, 11, 12), "Mustermann", 12345));
			Assert.AreEqual(1, m_Manager.GetBookingId(new Donation(100, new DateTime(2010, 11, 12), "Mustermann", 12346)));
		}

		[Test]
		public void AddSameDonationOnDifferentDay()
		{
			m_Manager.GetBookingId(new Donation(10, new DateTime(2010, 11, 12), "Mustermann", 12345));
			Assert.AreEqual(1, m_Manager.GetBookingId(new Donation(10, new DateTime(2010, 11, 13), "Mustermann", 12345)));
		}
	}
}

