using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using TntMPDConverter.Properties;
using System.Reflection;

namespace TntMPDConverter
{
	public class ProcessingDonations : State
	{
		public class Replacement
		{
			public string Donor;
		}

		public class NewDonor: Replacement
		{
			public int DonorNo;
		}

		protected static Dictionary<int, Dictionary<string, Replacement>> ReplacementInfo
		{
			get;
			set;
		}

		static ProcessingDonations()
		{
			var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase
				.Substring(Environment.OSVersion.Platform == PlatformID.Unix ? 7 : 8));
			ReplacementFileName = Path.GetFullPath(Path.Combine(dir, "replace.config"));
			UpdateReplacementInfo();
		}

		public ProcessingDonations(Scanner reader) : base(reader)
		{
		}

		protected static string ReplacementFileName
		{
			get;
			set;
		}

		protected static void UpdateReplacementInfo()
		{
			ReplacementInfo = new Dictionary<int, Dictionary<string, Replacement>>();
			ReadReplacementFile();
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

		private Replacement GetReplacement(int key, string searchText)
		{
			return ReplacementInfo[key][searchText];
		}

		internal void ApplyReplacements(Donation donation)
		{
			if (ReplacementInfo.ContainsKey(donation.DonorNo))
			{
				foreach (var searchText in ReplacementInfo[donation.DonorNo].Keys)
				{
					if (donation.Donor.Contains(searchText))
					{
						var newDonor = GetReplacement(donation.DonorNo, searchText) as NewDonor;
						donation.DonorNo = newDonor.DonorNo;
						donation.Donor = newDonor.Donor;
						break;
					}
				}
			}
			if (ReplacementInfo.ContainsKey(-1))
			{
				foreach (var searchText in ReplacementInfo[-1].Keys)
				{
					if (donation.Donor.Contains(searchText))
					{
						donation.Donor = GetReplacement(-1, searchText).Donor;
					}
				}
			}
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
							ApplyReplacements(donation);
							return donation;
						}
					}
					line = Reader.ReadLine();
				}
			}
		}

		protected static void ReadReplacementFile()
		{
			if (!File.Exists(ReplacementFileName))
				return;

			using (var reader = new StreamReader(ReplacementFileName))
			{
				int currentDonorNo = -1;
				Dictionary<string, Replacement> replacements = null;
				for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
				{
					if (line == "[Replacements]")
					{
						replacements = new Dictionary<string, Replacement>();
						currentDonorNo = -1;
						ReplacementInfo.Add(currentDonorNo, replacements);
					}
					else if (line.StartsWith("["))
					{
						replacements = new Dictionary<string, Replacement>();
						currentDonorNo = Convert.ToInt32(line.Substring(1, line.Length - 2));
						ReplacementInfo.Add(currentDonorNo, replacements);
					}
					else if (replacements != null && line.Contains("="))
					{
						var parts = line.Split('=');
						if (parts.Length != 2)
							continue;
						var donorInfo = parts[1].Split(';');
						if (donorInfo.Length != 2 && currentDonorNo != -1)
							continue;

						if (currentDonorNo == -1)
							replacements.Add(parts[0], new Replacement { Donor = parts[1] });
						else
						{
							replacements.Add(parts[0], new NewDonor {
								DonorNo = Convert.ToInt32(donorInfo[0].Trim(' ')),
								Donor = donorInfo[1].Trim(' ').Trim('"') });
						}
					}
				}
			}
		}
	}
}

