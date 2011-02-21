using System;
using System.Globalization;
using System.Text.RegularExpressions;
namespace TntMPDConverter
{

	public class ProcessDonors : State
	{
		private readonly Regex donorInfo =
			new Regex(@"\t(?<no>[0-9]+)\t(?<name>[^\t]+)\t(?<street>[^\t]+)(\t(?<phones>[^\t]+))?\t(?<count>[0-9,]+)\t(?<dm>[0-9,]+)\t(?<eur>[0-9,]+)$");
		private readonly Regex cityInfo =
			new Regex(@"\t(?<city>[0-9]+ .+)");
		private readonly CultureInfo info = new CultureInfo("de-DE");

		public ProcessDonors(Scanner reader) : base(reader)
		{
		}

		public static bool IsDonors(string line)
		{
			return line == "Spender\x00fcbersicht";
		}

		public Donor NextDonor
		{
			get
			{
				var line = Reader.ReadLine();
				var secondLine = Reader.ReadLine();
				if (donorInfo.IsMatch(line))
				{
					var cityLine = secondLine;
					if (!cityInfo.IsMatch(secondLine))
						cityLine = Reader.ReadLine();
					var donorInfoMatch = donorInfo.Match(line);
					var cityMatch = cityInfo.Match(cityLine);
					var donorNo = Convert.ToInt32(donorInfoMatch.Groups["no"].Value);
					var count = Convert.ToInt32(donorInfoMatch.Groups["count"].Value);
					var amount = Convert.ToDecimal(donorInfoMatch.Groups["eur"].Value, info);
					var phoneNos = donorInfoMatch.Groups["phones"].Value;
					if (secondLine != cityLine)
						phoneNos += secondLine.Substring(1); // strip initial \t

					return new Donor(donorNo, donorInfoMatch.Groups["name"].Value,
						donorInfoMatch.Groups["street"].Value, cityMatch.Groups["city"].Value,
						phoneNos, count, amount);
				}
				return null;
			}
		}

	}
}

