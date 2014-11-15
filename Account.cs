// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Globalization;

namespace TntMPDConverter
{
	public class Account : State
	{
		protected int m_AccountNo;

		public Account(Scanner reader)
			: base(reader)
		{
			IsValid = true;
		}

		private static bool IsNumber(string numberString)
		{
			var cultureInfo = new CultureInfo("de-DE");
			try
			{
				Convert.ToDecimal(numberString, cultureInfo);
			}
			catch (FormatException)
			{
				return false;
			}
			return true;
		}

		public static bool IsAccount(string line)
		{
			// the account line has 4 or 5 parts:
			// 3215	Sonstige Einnahmen (steuerneutral)	0,00	50,00
			// or
			// 3215	Sonstige Einnahmen (steuerneutral)	50,00
			string[] textArray = line.Split(new[] { '\t' });
			if (textArray.Length < 4 || textArray.Length > 5 ||
				(textArray.Length == 5 && !IsNumber(textArray[3])) ||
				!IsNumber(textArray[textArray.Length - 1]))
			{
				return false;
			}

			return IsNumber(textArray[1]);
		}

		public override State NextState()
		{
			ProcessAccountNo();
			switch (m_AccountNo)
			{
				case 7100:
				case 7800:
				case 1191: // new account numbers starting 1/2011
				case 1197:
				case 1185:
				case 3220: // new account numbers starting 12/2011
				case 3231:
				case 3239:
				case 3241: // 3241 - Transfers from other organizations
					return new ProcessingDonations(Reader);
				case 8900:
				case 3215: // new account numbers starting 1/2011
					return new ProcessingOtherProceeds(Reader);
				case 715: // Member transfer
					return new ProcessingMemberTransfers(m_AccountNo, Reader);
				case 3224: // 3224 - Transfers from other WOs
					return new ProcessingOtherTransfers(m_AccountNo, Reader);
				default:
					return new IgnoreAccount(Reader);
			}
		}

		protected void ProcessAccountNo()
		{
			string line = Reader.ReadLine();
			if (!IsAccount(line))
				throw new ApplicationException();
			string[] textArray = line.Split(new[] { '\t' });
			m_AccountNo = Convert.ToInt32(textArray[1]);
		}
	}
}

