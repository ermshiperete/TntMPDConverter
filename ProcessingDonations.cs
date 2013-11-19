// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
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

		protected virtual Donation EndDonation(Donation donation)
		{
			if (donation != null)
				Replacements.ApplyReplacements(donation);
			return donation;
		}

		protected virtual int NumberOfFields { get { return 7; }}
		protected virtual int DonorIndex { get { return 6; }}

		protected virtual bool IsContinuationLine(string[] partsOfLine)
		{
			return string.IsNullOrEmpty(partsOfLine[1]) || string.IsNullOrEmpty(partsOfLine[2])
				|| string.IsNullOrEmpty(partsOfLine[3]);
		}

		protected virtual Donation CreateDonation(string[] partsOfLine, CultureInfo cultureInfo)
		{
			var donation = new Donation {
				DonorNo = Convert.ToUInt32(partsOfLine[1]),
				Date = Convert.ToDateTime(partsOfLine[2], cultureInfo),
				Amount = Convert.ToDecimal(partsOfLine[3], cultureInfo)
			};
			if (partsOfLine[4] == "S")
			{
				donation.Amount = -donation.Amount;
			}
			donation.Donor = partsOfLine[6];
			return donation;
		}

		protected virtual void AppendSecondLine(Donation donation, string[] partsOfLine)
		{
			if (donation != null)
				donation.Donor = string.Format("{0} {1}", donation.Donor, partsOfLine[DonorIndex]);
		}

		public virtual Donation NextDonation
		{
			get
			{
				var cultureInfo = new CultureInfo("de-DE");
				Donation donation = null;
				string line = Reader.ReadLine();
				while (!(line == null && Reader.EndOfStream))
				{
					if (line.Trim() != "")
					{
						string[] partsOfLine = line.Split(new[] { '\t' });
						if (partsOfLine.Length != NumberOfFields)
						{
							if (IsEndOfDonation(line, partsOfLine[0]))
								return EndDonation(donation);
						}
						else if (IsContinuationLine(partsOfLine))
						{
							AppendSecondLine(donation, partsOfLine);
						}
						else
						{
							if (donation != null)
							{
								Reader.UnreadLine(line);
								donation = EndDonation(donation);
								// It's possible that a filter ignores this donation. In that
								// case we want to continue.
								if (donation != null)
									return donation;
							}

							donation = CreateDonation(partsOfLine, cultureInfo);
						}
					}
					line = Reader.ReadLine();
				}
				return EndDonation(donation);
			}
		}
	}
}

