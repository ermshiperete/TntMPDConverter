using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;


namespace TntMPDConverter
{

	public class ProcessDonors : State
	{
		private readonly Regex donorInfo =
			new Regex(@"^\t(?<no>[0-9]+)\t(?<name>[^\t]+)\t(?<street>[^\t]+)(\t(?<phones>[^\t]+))?\t(?<count>[0-9]+)\t(?<eur>-?[0-9,.]+)(\t(?<dm>[0-9,.]+))?$");
		private readonly Regex cityInfo =
			new Regex(@"^\t\t(?<name>[^\t]*)(\t(((?<plz>[0-9]+) )?(?<city>\p{Lu}[^\t]+))?(\t(?<phone2>.*))?)?");
		private readonly Regex emailInfo =
			new Regex(@"^\t\t\t\t(?<email>[-!#$%&'*+/=?^_`{|}~.A-Za-z0-9]+@[-A-Za-z0-9.]+)");
		private readonly Regex emailInfoCont =
			new Regex(@"^\t\t\t\t(?<email>[-!#$%&'*+/=?^_`{|}~.A-Za-z0-9]+)");
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
					var donorInfoMatch = donorInfo.Match(line);
					var phoneNos = new List<string>();
					var phone = new StringBuilder();
					var phoneThisLine = donorInfoMatch.Groups["phones"].Value;
					if (!string.IsNullOrEmpty(phoneThisLine))
						phone.Append(phoneThisLine);
					var contactPerson = new StringBuilder();
					var city = new StringBuilder();
					var plz = string.Empty;
					while (true)
					{
						if (string.IsNullOrEmpty(secondLine) || donorInfo.IsMatch(secondLine) ||
							!cityInfo.IsMatch(secondLine) || emailInfo.IsMatch(secondLine))
						{
							Reader.UnreadLine(secondLine);
							break;
						}
						var cityMatch = cityInfo.Match(secondLine);
						var nameThisLine = cityMatch.Groups["name"].Value;
						if (!string.IsNullOrEmpty(nameThisLine))
						{
							if (contactPerson.Length > 0)
								contactPerson.Append(" ");
							contactPerson.Append(nameThisLine);
						}
						if (!string.IsNullOrEmpty(cityMatch.Groups["plz"].Value))
							plz = cityMatch.Groups["plz"].Value;
						city.Append(cityMatch.Groups["city"].Value);
						phoneThisLine = cityMatch.Groups["phone2"].Value;
						if (string.IsNullOrEmpty(phoneThisLine))
						{
							if (phone.Length > 0)
								phone.AppendLine();
						}
						else
						{
							if (phoneThisLine.StartsWith("0") || phoneThisLine.StartsWith("+"))
								phone.AppendLine();
							phone.Append(phoneThisLine);
						}
						secondLine = Reader.ReadLineFiltered();
					}
					var donorNo = Convert.ToUInt32(donorInfoMatch.Groups["no"].Value);
					var count = Convert.ToInt32(donorInfoMatch.Groups["count"].Value);
					var amount = Convert.ToDecimal(donorInfoMatch.Groups["eur"].Value, info);

					var email = new StringBuilder();
					bool firstEmailLineRead = false;
					while (true)
					{
						var thirdLine = Reader.ReadLineFiltered();
						if (string.IsNullOrEmpty(thirdLine) || donorInfo.IsMatch(thirdLine) ||
							!(emailInfo.IsMatch(thirdLine) ||
								(firstEmailLineRead && emailInfoCont.IsMatch(thirdLine))))
						{
							Reader.UnreadLine(thirdLine);
							break;
						}
						Match emailMatch;
						if (emailInfo.IsMatch(thirdLine))
							emailMatch = emailInfo.Match(thirdLine);
						else
							emailMatch = emailInfoCont.Match(thirdLine);
						email.Append(emailMatch.Groups["email"].Value);
						firstEmailLineRead = true;
					}

					if (!string.IsNullOrEmpty(phone.ToString()))
					{
						foreach (var thisPhone in phone.ToString().Split('\n'))
						{
							if (!string.IsNullOrEmpty(thisPhone))
								phoneNos.Add(thisPhone);
						}
					}

					return new Donor(donorNo, donorInfoMatch.Groups["name"].Value, contactPerson.ToString(),
						donorInfoMatch.Groups["street"].Value, plz,
						city.ToString(), phoneNos.ToArray(), email.ToString(),
						count, amount);
				}
				return null;
			}
		}

	}
}

