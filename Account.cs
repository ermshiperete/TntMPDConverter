// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;

namespace TntMPDConverter
{
	public class Account : State
	{
		private int m_AccountNo;

		public Account(Scanner reader) : base(reader)
		{
			IsValid = true;
		}

		public static bool IsAccount(string line)
		{
			string[] textArray1 = line.Split(new[] { '\t' });
			if (textArray1.Length != 4)
			{
				return false;
			}
			try
			{
				Convert.ToInt32(textArray1[1]);
			}
			catch (FormatException)
			{
				return false;
			}
			return true;
		}

		public override State NextState()
		{
			ProcessAccountNo();
			if (m_AccountNo == 7100 || m_AccountNo == 7800 ||
				// new account numbers starting 1/2011
				m_AccountNo == 1191 || m_AccountNo == 1197 || m_AccountNo == 1185 ||
				// new account numbers starting 12/2011
				m_AccountNo == 3220 || m_AccountNo == 3231 || m_AccountNo == 3239 ||
				// 3241 - Transfers from other organizations
				m_AccountNo == 3241)
			{
				return new ProcessingDonations(Reader);
			}
			if (m_AccountNo == 8900 ||
				// new account numbers starting 1/2011
				m_AccountNo == 3215)
			{
				return new ProcessingOtherProceeds(Reader);
			}
			// Member transfer
			if (m_AccountNo == 715)
			{
				return new ProcessingMemberTransfers(m_AccountNo, Reader);
			}
			// 3224 - Transfers from other WOs
			if (m_AccountNo == 3224)
			{
				return new ProcessingOtherTransfers(m_AccountNo, Reader);
			}
			return new IgnoreAccount(Reader);
		}

		protected void ProcessAccountNo()
		{
			string text1 = Reader.ReadLine();
			string[] textArray1 = text1.Split(new[] { '\t' });
			if (textArray1.Length != 4)
			{
				throw new ApplicationException();
			}
			text1 = Reader.ReadLine();
			Reader.UnreadLine(text1);
			IsValid = IsAccount(text1);
			m_AccountNo = Convert.ToInt32(textArray1[1]);
		}
	}
}

