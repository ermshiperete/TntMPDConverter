// Copyright (c) 2014, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using NUnit.Framework;
using System;

namespace TntMPDConverter
{
	[TestFixture]
	public class AccountTests
	{
		class DummyAccount: Account
		{
			public DummyAccount(Scanner reader)
				: base(reader)
			{
			}

			public int Account
			{
				get
				{
					ProcessAccountNo();
					return m_AccountNo;
				}
			}
		}

		[Test]
		public void IsAccount_4parts()
		{
			Assert.That(Account.IsAccount("	3215	Sonstige Einnahmen (steuerneutral)	50,00"), Is.True);
		}

		[Test]
		public void IsAccount_5parts()
		{
			Assert.That(Account.IsAccount("	3215	Sonstige Einnahmen (steuerneutral)	0,00	50,00"), Is.True);
		}

		[Test]
		public void AccountNumber_4parts()
		{
			var reader = new FakeScanner(@"	3215	Sonstige Einnahmen (steuerneutral)	50,00");
			var account = new DummyAccount(reader);
			Assert.That(account.Account, Is.EqualTo(3215));
		}

		[Test]
		public void AccountNumber_5parts()
		{
			var reader = new FakeScanner(@"	3215	Sonstige Einnahmen (steuerneutral)	0,00	50,00");
			var account = new DummyAccount(reader);
			Assert.That(account.Account, Is.EqualTo(3215));
		}
	}
}

