namespace TntMPDConverter
{
	using System;

	public class Donation
	{
		private int m_Id;
		private static DonationManager s_Manager = new DonationManager();

		/// <summary>
		/// Resets the donation manager. To be used in tests.
		/// </summary>
		public static void Reset()
		{
			s_Manager = new DonationManager();
		}

		public Donation()
		{
		}
		public Donation(decimal amount, DateTime date, string donor, int donorNo)
		{
			Amount = amount;
			Date = date;
			DonorNo = donorNo;
			Donor = donor;
		}

		public decimal Amount { get; set; }
		public DateTime Date { get; set; }
		public string Donor { get; set; }
		public int DonorNo {get; set;}

		private int Id
		{
			get
			{
				if (m_Id == 0)
				{
					m_Id = s_Manager.GetBookingId(this);
				}
				return m_Id;
			}
		}

		public string BookingId
		{
			get
			{
				return string.Format("{0}{1:d2}{2:d2}{3:d5}{4}", Date.Year, Date.Month, Date.Day, DonorNo, Id);
			}
		}

		public override string ToString()
		{
			return string.Format("[Donation: Amount={0}, Date={1}, Donor={2}, DonorNo={3}, BookingId={4}]", Amount, Date, Donor, DonorNo, BookingId);
		}
	}
}

