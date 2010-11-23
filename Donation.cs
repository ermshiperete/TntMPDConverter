namespace TntMPDConverter
{
    using System;

    public class Donation
    {
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

        public decimal Amount;
        public string BookingId;
        public DateTime Date;
        public string Donor;
        public int DonorNo;
    }
}

