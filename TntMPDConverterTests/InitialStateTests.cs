using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TntMPDConverter;

namespace TntMPDConverterTests
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class InitialStateTests
	{
		private Scanner m_Reader;

		[SetUp]
		public void SetUp()
		{
			m_Reader = new FakeScanner(
"Projekt\t301234  Mustermann, Markus\tSoll €\tHaben €\n" +
"Projektabrechnung\n" +
"Erstellung:\t15.10.2009\n" +
"Projekt\t301234  Mustermann, Markus\n" +
"Zeitraum:\t01.09.2009 - 30.09.2009\n" +
"\tErträge\tSoll €\tHaben €\n" +
"\t7100\tSpenden (wiss.) Arbeit\t3.694,59\n" +"\t16747\t01.09.2009\t10,23\tH\tKD \tMerkel, Angela");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the first real state that is returned is the Project state
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ReturnsProjectState()
		{
			Assert.IsInstanceOf(typeof(Project), new Initial(m_Reader).NextState());
		}
	}
}
