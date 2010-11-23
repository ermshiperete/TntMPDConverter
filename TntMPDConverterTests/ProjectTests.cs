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
	public class ProjectTests
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
"\t7100\tSpenden (wiss.) Arbeit\t3.694,59\n" +
"\t16747\t01.09.2009\t10,23\tH\tKD \tMerkel, Angela");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that project state recognizes the project
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void IsProject()
		{
			Assert.IsTrue(Project.IsProject("Projekt\t301234  Mustermann, Markus\tSoll €\tHaben €"));
		}

		/// <summary>
		/// Tests that the next state is Account
		/// </summary>
		[Test]
		public void NextState()
		{
			Assert.IsInstanceOf(typeof(Account), new Project(m_Reader).NextState());
		}

		/// <summary>
		/// Tests that the project number was recognized correctly
		/// </summary>
		[Test]
		public void ProjectNo()
		{
			var project = new Project(m_Reader);
			project.NextState();
			Assert.AreEqual(301234, project.ProjectNo);
		}
	}
}
