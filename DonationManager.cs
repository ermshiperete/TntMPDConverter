using System;
using System.Collections.Generic;
namespace TntMPDConverter
{
	public class DonationManager
	{
		private class DonationInfo
		{
			public DonationInfo(DateTime date, int donationNo)
			{
				Date = date;
				DonationNo = donationNo;
			}

			public DateTime Date { get; private set; }
			public int DonationNo { get; private set; }

			public override bool Equals(object obj)
			{
				var other = obj as DonationInfo;
				if (other == null)
					return false;

				return Date == other.Date && DonationNo == other.DonationNo;
			}

			public override int GetHashCode()
			{
				return Date.GetHashCode() ^ DonationNo.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("[DonationInfo: Date={0}, DonationNo={1}]", Date, DonationNo);
			}
		}

		private Dictionary<DonationInfo, List<decimal>> m_Donations = new Dictionary<DonationInfo, List<decimal>>();

		public DonationManager()
		{
		}

		private DonationInfo GetDonationInfo(Donation donation)
		{
			return new DonationInfo(donation.Date, donation.DonorNo);
		}

		public int GetBookingId(Donation donation)
		{
			var donationInfo = GetDonationInfo(donation);
			if (m_Donations.ContainsKey(donationInfo))
			{
				m_Donations[donationInfo].Add(donation.Amount);
				return m_Donations[donationInfo].Count;
			}
			m_Donations.Add(donationInfo, new List<decimal> { donation.Amount });
			return 1;
		}
	}
}

