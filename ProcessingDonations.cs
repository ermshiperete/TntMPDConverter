using System;
using System.Collections.Generic;
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

		protected Dictionary<int, Dictionary<string, NewDonor>> ReplacementInfo { get; set; }

		public ProcessingDonations(Scanner reader) : base(reader)
        {
        	var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase
				.Substring(Environment.OSVersion.Platform == PlatformID.Unix ? 7 : 8));
        	ReplacementFileName = Path.GetFullPath(Path.Combine(dir, "replacement.config"));
			UpdateReplacementInfo();
        }

		protected string ReplacementFileName { get; set; }

		protected void UpdateReplacementInfo()
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

        public virtual Donation NextDonation
        {
            get
            {
                var info1 = new CultureInfo("de-DE");
                Donation donation = null;
                string line = Reader.ReadLine();
                while (true)
                {
                    if (line.Trim() != "")
                    {
                        string[] textArray1 = line.Split(new[] { '\t' });
                        if (textArray1.Length != 7)
                        {
                            if (!textArray1[0].StartsWith("Projekt"))
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
                                        		DonorNo = Convert.ToInt32(textArray1[1]),
                                        		Date = Convert.ToDateTime(textArray1[2], info1),
                                        		Amount = Convert.ToDecimal(textArray1[3], info1)
                                        	};
                        	if (textArray1[4] == "S")
                            {
                                donation.Amount = -donation.Amount;
                            }
                            var settings = Settings.Default;
                            settings.BookingId++;
                            donation.BookingId = Settings.Default.BookingId.ToString();
                            donation.Donor = textArray1[6];
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

		protected void ReadReplacementFile()
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

