// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
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
			public uint DonorNo;
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

		protected static Dictionary<int, List<string>> IncludeInfo
		{
			get;
			set;
		}

		protected static Dictionary<int, List<string>> ExcludeInfo { get; set; }

		protected static string ReplacementFileName
		{
			get;
			set;
		}

		protected static void UpdateReplacementInfo()
		{
			ReplacementInfo = new Dictionary<int, Dictionary<string, Replacement>>();
			IncludeInfo = new Dictionary<int, List<string>>();
			ExcludeInfo = new Dictionary<int, List<string>>();
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

		private bool ApplyRegexReplacements(int index, Donation donation)
		{
			if (ReplacementInfo.ContainsKey(index))
			{
				foreach (var pattern in ReplacementInfo[index].Keys)
				{
					donation.Donor = Regex.Replace(donation.Donor, pattern,
						GetReplacement(index, pattern).Donor);
				}
			}
			return !string.IsNullOrEmpty(donation.Donor);
		}

		private bool ApplyDonationReplacements(Donation donation)
		{
			if (ReplacementInfo.ContainsKey((int)donation.DonorNo))
			{
				foreach (var searchText in ReplacementInfo[(int)donation.DonorNo].Keys)
				{
					if (donation.Donor.Contains(searchText))
					{
						var newDonor = GetReplacement((int)donation.DonorNo, searchText) as NewDonor;
						donation.DonorNo = newDonor.DonorNo;
						donation.Donor = newDonor.Donor;
						break;
					}
				}
			}
			return !string.IsNullOrEmpty(donation.Donor);
		}

		private bool ApplyStringReplacements(Donation donation)
		{
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
			return !string.IsNullOrEmpty(donation.Donor);
		}

		public void ApplyReplacements(Donation donation)
		{
			if (!ApplyDonationReplacements(donation))
				return;

			if (!ApplyStringReplacements(donation))
				return;

			ApplyRegexReplacements(RegexReplacements, donation);
		}

		private bool IncludeExcludeEntry(int accountNo, string entry, Dictionary<int, List<string>> info)
		{
			if (!info.ContainsKey(accountNo))
				return false;

			foreach (var pattern in info[accountNo])
			{
				if (Regex.IsMatch(entry, pattern))
					return true;
			}
			return false;
		}

		public bool IncludeEntry(int accountNo, string entry)
		{
			return IncludeExcludeEntry(accountNo, entry, IncludeInfo);
		}

		public bool ExcludeEntry(int accountNo, string entry)
		{
			return IncludeExcludeEntry(accountNo, entry, ExcludeInfo);
		}

		protected static void ReadReplacementFile()
		{
			if (!File.Exists(ReplacementFileName))
				return;

			using (var reader = new StreamReader(ReplacementFileName))
			{
				int currentDonorNo = Replacements;
				Dictionary<string, Replacement> replacements = null;
				List<string> includesExcludes = null;
				bool processingIncludes = false;
				string lastPattern = string.Empty;
				for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
				{
					if (line == "[Replacements]")
					{
						replacements = new Dictionary<string, Replacement>();
						includesExcludes = null;
						currentDonorNo = Replacements;
						ReplacementInfo.Add(currentDonorNo, replacements);
					}
					else if (line == "[Regex]")
					{
						replacements = new Dictionary<string, Replacement>();
						includesExcludes = null;
						currentDonorNo = RegexReplacements;
						ReplacementInfo.Add(currentDonorNo, replacements);
					}
					else if (line.StartsWith("[K"))
					{
						replacements = null;
						includesExcludes = new List<string>();
						currentDonorNo = Convert.ToInt32(line.Substring(2, line.Length - 3));
						if (currentDonorNo == 715)
						{
							IncludeInfo.Add(currentDonorNo, includesExcludes);
							processingIncludes = true;
						}
						else
						{
							ExcludeInfo.Add(currentDonorNo, includesExcludes);
							processingIncludes = false;
						}
					}
					else if (line.StartsWith("["))
					{
						replacements = new Dictionary<string, Replacement>();
						includesExcludes = null;
						currentDonorNo = Convert.ToInt32(line.Substring(1, line.Length - 2));
						ReplacementInfo.Add(currentDonorNo, replacements);
					}
					else if (line.Contains("="))
					{
						var parts = line.Split('=');
						if (parts.Length != 2)
							continue;
						if (replacements != null)
						{
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
										DonorNo = Convert.ToUInt32(donorInfo[0].Trim(' ')),
										Donor = donorInfo[1].Trim(' ').Trim('"') });
								}
							}
						}
						else if (includesExcludes != null && ((parts[0] == "Include" && processingIncludes) ||
							(parts[0] == "Exclude" && !processingIncludes)))
						{
							includesExcludes.Add(StripQuotes(parts[1]));
						}
					}
				}
			}
		}
	}
}

