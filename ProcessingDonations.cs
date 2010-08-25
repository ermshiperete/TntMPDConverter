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
		public class NewDonor
		{
			public int DonorNo;
			public string Donor;
		}

		protected static Dictionary<int, Dictionary<string, NewDonor>> ReplacementInfo { get; set; }

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

		protected static string ReplacementFileName { get; set; }

		protected static void UpdateReplacementInfo()
		{
			ReplacementInfo = new Dictionary<int, Dictionary<string, NewDonor>>();
			ReadReplacementFile();
		}

        public override State NextState()
        {
            string line = Reader.ReadLine();
            Reader.UnreadLine(line);
            return CheckForStartOfNewState(line) ?? new Ignore(Reader);
        }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                            if (!strings[0].StartsWith("Projekt") && !strings[0].StartsWith("\fProjekt"))
                            {
                                Reader.UnreadLine(line);
                                IsValid = false;
                                return donation;
                            }
                        }
                        else
                        {
                            donation = new Donation
                                        	{
                                        		DonorNo = Convert.ToInt32(strings[1]),
                                        		Date = Convert.ToDateTime(strings[2], info),
                                        		Amount = Convert.ToDecimal(strings[3], info)
                                        	};
                        	if (strings[4] == "S")
                            {
                                donation.Amount = -donation.Amount;
                            }
                            var settings = Settings.Default;
                            settings.BookingId++;
                            donation.BookingId = Settings.Default.BookingId.ToString();
                            donation.Donor = strings[6];
							if (ReplacementInfo.ContainsKey(donation.DonorNo))
							{
								foreach (var key in ReplacementInfo[donation.DonorNo].Keys)
								{
									if (donation.Donor.Contains(key))
									{
										var newDonor = ReplacementInfo[donation.DonorNo][key];
										donation.DonorNo = newDonor.DonorNo;
										donation.Donor = newDonor.Donor;
										break;
									}
								}
							}
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
				Dictionary<string, NewDonor> newDonors = null;
				for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
				{
					if (line.StartsWith("["))
					{
						newDonors = new Dictionary<string, NewDonor>();
						ReplacementInfo.Add(Convert.ToInt32(line.Substring(1, line.Length - 2)), newDonors);
					}
					else if (newDonors != null && line.Contains("="))
					{
						var parts = line.Split('=');
						if (parts.Length != 2)
							continue;
						var donorInfo = parts[1].Split(';');
						if (donorInfo.Length != 2)
							continue;
						newDonors.Add(parts[0], new NewDonor {DonorNo = Convert.ToInt32(donorInfo[0].Trim(' ')), 
							Donor = donorInfo[1].Trim(' ').Trim('"')});
					}
				}
			}
		}
    }
}

