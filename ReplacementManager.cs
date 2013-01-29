using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TntMPDConverter
{
	public class ReplacementManager
	{
		public class Replacement
		{
			public string Donor;
		}

		public class NewDonor: Replacement
		{
			public int DonorNo;
		}

		private const int Replacements = -1;
		private const int RegexReplacements = -2;

		static ReplacementManager()
		{
			var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase
				.Substring(Environment.OSVersion.Platform == PlatformID.Unix ? 7 : 8));
			ReplacementFileName = Path.GetFullPath(Path.Combine(dir, "replace.config"));
			UpdateReplacementInfo();
		}

		protected static Dictionary<int, Dictionary<string, Replacement>> ReplacementInfo
		{
			get;
			set;
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

		private Replacement GetReplacement(int key, string searchText)
		{
			return ReplacementInfo[key][searchText];
		}

		private static string StripQuotes(string text)
		{
			if (text.StartsWith("\""))
				text = text.Substring(1);
			if (text.EndsWith("\""))
				text = text.Substring(0, text.Length - 1);
			return text;
		}

		public void ApplyReplacements(Donation donation)
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
			if (ReplacementInfo.ContainsKey(Replacements))
			{
				foreach (var searchText in ReplacementInfo[Replacements].Keys)
				{
					if (donation.Donor.Contains(searchText))
					{
						donation.Donor = GetReplacement(Replacements, searchText).Donor;
					}
				}
			}
			if (ReplacementInfo.ContainsKey(RegexReplacements))
			{
				foreach (var pattern in ReplacementInfo[RegexReplacements].Keys)
				{
					donation.Donor = Regex.Replace(donation.Donor, pattern,
						GetReplacement(RegexReplacements, pattern).Donor);
				}
			}
		}

		protected static void ReadReplacementFile()
		{
			if (!File.Exists(ReplacementFileName))
				return;

			using (var reader = new StreamReader(ReplacementFileName))
			{
				int currentDonorNo = Replacements;
				Dictionary<string, Replacement> replacements = null;
				string lastPattern = string.Empty;
				for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
				{
					if (line == "[Replacements]")
					{
						replacements = new Dictionary<string, Replacement>();
						currentDonorNo = Replacements;
						ReplacementInfo.Add(currentDonorNo, replacements);
					}
					else if (line == "[Regex]")
					{
						replacements = new Dictionary<string, Replacement>();
						currentDonorNo = RegexReplacements;
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
						if (currentDonorNo == RegexReplacements)
						{
							if (parts[0] == "Pattern")
								lastPattern = StripQuotes(parts[1]);
							else if (parts[0] == "Replace" && !string.IsNullOrEmpty(lastPattern))
							{
								replacements.Add(lastPattern, new Replacement { Donor = StripQuotes(parts[1]) });
								lastPattern = string.Empty;
							}
							else
								lastPattern = string.Empty;
						}
						else
						{
							var donorInfo = parts[1].Split(';');
							if (donorInfo.Length != 2 && currentDonorNo != -1)
								continue;

							if (currentDonorNo == Replacements)
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
}

