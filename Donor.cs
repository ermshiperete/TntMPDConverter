// Copyright (c) 2011, Eberhard Beilharz. All Rights Reserved.
using System;
using System.Text;
namespace TntMPDConverter
{
	public class Donor
	{
		// possible Titles, based on real data in database as of 2011-05-30
		private readonly string[] Titles = new string[] {
			"Dekan i.R.",
			"Dipl.-Finanzwirt",
			"Dipl.-Ing.",
			"Dipl.-Phys.",
			"Dr.",
			"Dr. Ing.",
			"Dr. med.",
			"Dres.med.",
			"Drs.",
			"Pastor",
			"Pastorin",
			"Pfr.",
			"Pfr. i.R.",
			"Prof.",
			"Prof. Dr.",
			"Prof. Dr. Dr. h.c.",
			"Prof. Dr. Ing.",
			"Pfarrer",
			"Dres.",
			"Pfarrerin",
			"Dipl.-Ing.(FH)",
			"Mag.",
		};

		// possible strings that separate name of spouses
		private readonly string[] Separators = new string[] {
			" und ",
			" u. ",
			" & ",
			" + "
		};

		public Donor()
		{
		}

		public Donor(int donorNo, string name, string street, string plz, string city,
			string[] phoneNos, string email, int donationsCount, decimal amount)
		{
			DonorNo = donorNo;
			Name = name;
			Street = street;
			Plz = plz;
			City = city;
			PhoneNos = phoneNos;
			Email = email;
			DonationsCount = donationsCount;
			Amount = amount;
		}

		public int DonorNo { get; private set; }
		public string Name { get; private set; }
		public string Street { get; private set; }
		public string Plz { get; private set; }
		public string City { get; private set; }
		public string[] PhoneNos { get; private set; }
		public string Email { get; private set; }
		public int DonationsCount { get; private set; }
		public decimal Amount { get; private set; }
		public string CombinedPhoneNo
		{
			get
			{
				var bldr = new StringBuilder();
				foreach (var phone in PhoneNos)
				{
					var str = phone;
					if (bldr.Length > 0)
						bldr.Append(", ");
					if (phone.StartsWith("p:"))
						str = phone.Substring(2).TrimStart();
					bldr.Append(str);
				}
				return bldr.ToString();
			}
		}
		
		private string SplitLast(string name, out string rest)
		{
			rest = string.Empty;
			if (!string.IsNullOrEmpty(name))
			{
				var index = name.IndexOf(',');
				if (index > 0)
				{
					rest = name.Substring(index + 1).TrimStart();
					return name.Substring(0, index);
				}
			}
			return name;
		}

		private string SplitSpouse(string name, out string spouseLast, out string spouseFirst)
		{
			spouseLast = string.Empty;
			spouseFirst = string.Empty;
			int index1 = -1;
			int index2 = 0;
			foreach (var separator in Separators)
			{
				if (name.Contains(separator))
				{
					index1 = name.IndexOf(separator);
					index2 = index1 + separator.Length;
					break;
				}
			}

			if (index1 > -1)
			{
				var spouse = name.Substring(index2);
				if (spouse.IndexOf(',') > 0)
					spouseLast = SplitLast(spouse, out spouseFirst);
				else
					spouseFirst = spouse;
				return name.Substring(0, index1);
			}
			return name;
		}

		private string SplitFirstTitle(string name, out string title)
		{
			title = string.Empty;
			foreach (var t in Titles)
			{
				if (name.StartsWith(t) && t.Length > title.Length)
				{
					title = t;
				}
			}
			if (title.Length > 0)
				return name.Substring(title.Length).TrimStart();

			return name;
		}

		public string LastName
		{
			get
			{
				string rest;
				return SplitLast(Name, out rest);
			}
		}

		public string FirstName
		{
			get
			{
				string title, rest1, rest2;
				SplitLast(Name, out rest1);
				return SplitFirstTitle(SplitSpouse(rest1, out rest1, out rest2), out title);
			}
		}

		public string PersonType
		{
			get
			{
				return string.IsNullOrEmpty(FirstName) ? "O" : "P";
			}
		}

		public string Title
		{
			get
			{
				string title, rest;
				SplitLast(Name, out rest);
				SplitFirstTitle(rest, out title);
				return title;
			}
		}

		public string SpouseLastName
		{
			get
			{
				string rest, spouseLast, spouseFirst;
				SplitLast(Name, out rest);
				SplitSpouse(rest, out spouseLast, out spouseFirst);
				return spouseLast;
			}
		}

		public string SpouseFirstName
		{
			get
			{
				string title, rest, spouseLast, spouseRest;
				SplitLast(Name, out rest);
				SplitSpouse(rest, out spouseLast, out spouseRest);
				return SplitFirstTitle(spouseRest, out title);
			}
		}

		public string SpouseTitle
		{
			get
			{
				string title, rest, spouseLast, spouseRest;
				SplitLast(Name, out rest);
				SplitSpouse(rest, out spouseLast, out spouseRest);
				SplitFirstTitle(spouseRest, out title);
				return title;
			}
		}
	}
}

