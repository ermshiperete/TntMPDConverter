using System;
using System.Globalization;

namespace TntMPDConverter
{
	public class ProcessingDonations : State
	{
		protected ReplacementManager Replacements;

		public ProcessingDonations(Scanner reader) : base(reader)
		{
			Replacements = new ReplacementManager();
		}

		public override State NextState()
		{
			string line = Reader.ReadLine();
			Reader.UnreadLine(line);
			return CheckForStartOfNewState(line) ?? new Ignore(Reader);
		}

		internal bool IsEndOfDonation(string line, string startOfLine)
		{
			if (!startOfLine.StartsWith("Projekt") && !startOfLine.StartsWith("\fProjekt"))
			{
				Reader.UnreadLine(line);
				IsValid = false;
				return true;
			}
			return false;
		}

		public virtual Donation NextDonation
		{
			get
			{
				var info = new CultureInfo("de-DE");
				Donation donation = null;
				string line = Reader.ReadLine();
				while (true)
				{
					if (line.Trim() != "")
					{
						string[] strings = line.Split(new[] { '\t' });
						if (strings.Length != 7)
						{
							if (IsEndOfDonation(line, strings[0]))
								return donation;
						}
						else
						{
							donation = new Donation { DonorNo = Convert.ToInt32(strings[1]),
								Date = Convert.ToDateTime(strings[2], info),
								Amount = Convert.ToDecimal(strings[3], info) };
							if (strings[4] == "S")
							{
								donation.Amount = -donation.Amount;
							}
							donation.Donor = strings[6];
							Replacements.ApplyReplacements(donation);
							return donation;
						}
					}
					line = Reader.ReadLine();
				}
			}
		}
	}
}

