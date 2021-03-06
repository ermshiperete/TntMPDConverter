// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;

namespace TntMPDConverter
{
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
		public Donation(decimal amount, DateTime date, string donor, uint donorNo)
		{
			Amount = amount;
			Date = date;
			DonorNo = donorNo;
			Donor = donor;
		}
		public Donation(decimal amount, DateTime date, string donor, uint donorNo, string remarks)
			: this(amount, date, donor, donorNo)
		{
			Remarks = remarks;
		}

		public Donation(decimal amount, DateTime date, string donor, uint donorNo, string remarks,
			decimal tenderedAmount, string tenderedCurrency): this(amount, date, donor, donorNo, remarks)
		{
			TenderedAmount = tenderedAmount;
			TenderedCurrency = tenderedCurrency;
		}

		public decimal Amount { get; set; }
		public DateTime Date { get; set; }
		public string Donor { get; set; }
		public uint DonorNo {get; set;}
		public string Remarks { get; set; }
		public decimal TenderedAmount { get; set; }
		public string TenderedCurrency { get; set; }

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
				var s = string.Format("{0}{1:d2}{2:d2}{3:d5}{4}", Date.Year, Date.Month,
					Date.Day, DonorNo, Id);
				// Max length of booking id that TntMPD accepts is 18 characters
				if (s.Length > 18)
					s = s.Substring(0, 17) + Id.ToString();
				return s;
			}
		}

		public override string ToString()
		{
			return string.Format("[Donation: Amount={0}, Date={1}, Donor={2}, DonorNo={3}, BookingId={4}]", Amount, Date, Donor, DonorNo, BookingId);
		}
	}
}

