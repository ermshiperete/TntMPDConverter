using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
namespace TntMPDConverter
{

	public class ProcessDonors : State
	{
		private readonly Regex donorInfo =
			new Regex(@"\t(?<no>[0-9]+)\t(?<name>[^\t]+)\t(?<street>[^\t]+)(\t(?<phones>[^\t]+))?\t(?<count>[0-9]+)\t(?<eur>[0-9,.]+)(\t(?<dm>[0-9,.]+))?$");
		private readonly Regex cityInfo =
			new Regex(@"\t(?<plz>[0-9]+) (?<city>[^\t]+)(\t(?<phone2>.+))?");
		private readonly CultureInfo info = new CultureInfo("de-DE");

		public ProcessDonors(Scanner reader) : base(reader)
		{
		}

		public static bool IsDonors(string line)
		{
			return line == "Spender\x00fcbersicht";
		}

		public override State NextState()
		{
			if (Reader.EndOfStream)
				return base.NextState();
			return this;
		}

		private string FirstDonorLine()
		{
			string line;
			for (line = Reader.ReadLineFiltered();
				!string.IsNullOrEmpty(line) && !donorInfo.IsMatch(line);
				line = Reader.ReadLineFiltered())
				;
			return line;
		}

		public Donor NextDonor
		{
			get
			{
				var line = FirstDonorLine();
				var secondLine = Reader.ReadLineFiltered();
				if (!string.IsNullOrEmpty(line) && donorInfo.IsMatch(line))
				{
					var cityLine = secondLine;
					if (string.IsNullOrEmpty(secondLine) || !cityInfo.IsMatch(secondLine))
						cityLine = Reader.ReadLineFiltered();
					var donorInfoMatch = donorInfo.Match(line);
					var cityMatch = cityInfo.Match(cityLine);
					var donorNo = Convert.ToInt32(donorInfoMatch.Groups["no"].Value);
					var count = Convert.ToInt32(donorInfoMatch.Groups["count"].Value);
					var amount = Convert.ToDecimal(donorInfoMatch.Groups["eur"].Value, info);
					var phoneNos = new List<string>();
					var phone = donorInfoMatch.Groups["phones"].Value;
					if (!string.IsNullOrEmpty(phone))
					{
						phoneNos.Add(phone);
						if (secondLine != cityLine)
						{
							// strip initial \t
							phoneNos[0] += secondLine.TrimStart('\t');
						}
					}
					var phone2 = cityMatch.Groups["phone2"].Value;
					if (!string.IsNullOrEmpty(phone2))
						phoneNos.Add(phone2);

					var email = string.Empty;
					var thirdLine = Reader.ReadLineFiltered();
					if (string.IsNullOrEmpty(thirdLine) || donorInfo.IsMatch(thirdLine))
						Reader.UnreadLine(thirdLine);
					else
					{
						// strip initial \t
						email = thirdLine.TrimStart('\t');
					}
					return new Donor(donorNo, donorInfoMatch.Groups["name"].Value,
						donorInfoMatch.Groups["street"].Value, cityMatch.Groups["plz"].Value,
						cityMatch.Groups["city"].Value, phoneNos.ToArray(), email, count, amount);
				}
				return null;
			}
		}

	}
}

