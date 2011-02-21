// Copyright (c) 2011, Eberhard Beilharz. All Rights Reserved.
using System;
namespace TntMPDConverter
{
	public class Donor
	{
		public Donor()
		{
		}

		public Donor(int donorNo, string name, string street, string city, string phoneNos,
			int donationsCount, decimal amount)
		{
			DonorNo = donorNo;
			Name = name;
			Street = street;
			City = city;
			PhoneNos = phoneNos;
			DonationsCount = donationsCount;
			Amount = amount;
		}

		public int DonorNo { get; private set; }
		public string Name { get; private set; }
		public string Street { get; private set; }
		public string City { get; private set; }
		public string PhoneNos { get; private set; }
		public int DonationsCount { get; private set; }
		public decimal Amount { get; private set; }
	}
}

