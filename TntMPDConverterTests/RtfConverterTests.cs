// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System.IO;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class RtfConverterTests
	{
		private string m_TempFile;

		[SetUp]
		public void Setup()
		{
			m_TempFile = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(m_TempFile);
		}

		[Test]
		public void SimpleConversion()
		{
			File.WriteAllText(m_TempFile, "{\\rtf1\\ansi\\ansicpg65001\\deff0\\deflang1033{" +
				"\\fonttbl{\\f0\\fnil Arial;}}{\\colortbl\\red0\\green0\\blue0 ;}" +
				"{\\*\\generator NRtfTree Library 0.3.0;}\\uc1\\margl1133\\margr1133\\margt1133" +
				"\\margb1133\\viewkind4\\pard\\cf0\\fs40\\f0\\b\\ul First line\\line\\par Second line\\par}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo("First line\n\nSecond line\n"));
		}

		[Test]
		public void SimpleWithTabs()
		{
			File.WriteAllText(m_TempFile, @"{\rtf1\ansi\ansicpg65001\deff0\deflang1033{" +
				@"\fonttbl{\f0\fnil Arial;}}{\colortbl\red0\green0\blue0 ;}" +
				@"{\*\generator NRtfTree Library 0.3.0;}\uc1\margl1133\margr1133\margt1133" +
				@"\margb1133\viewkind4" +
				@"\pard\plain\sb82\tx1362{\plain\tab\fs14\f3\cf0\cb1 First\plain\tab\fs14\f3\cf0\cb1 Line{\fs18\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo("\tFirst\tLine\n"));
		}

		[Test]
		public void TwoLineWithTabs()
		{
			File.WriteAllText(m_TempFile, @"{\rtf1\ansi\ansicpg65001\deff0\deflang1033{" +
				@"\fonttbl{\f0\fnil Arial;}}{\colortbl\red0\green0\blue0 ;}" +
				@"{\*\generator NRtfTree Library 0.3.0;}\uc1\margl1133\margr1133\margt1133" +
				@"\margb1133\viewkind4" +
				@"\pard\plain\sb82\tx1362\tqr\tx2785\tqr\tx3334\tqr\tx4220\tx4310\tx4466\tx5443" +
				@"{\plain\tab\fs14\f3\cf0\cb1 One\plain\tab\fs14\f3\cf0\cb1 Two\plain\tab\fs14\f3\cf0\cb1 Three" +
				@"\plain\tab\fs14\f3\cf0\cb1 Four\plain\tab\fs14\f3\cf0\cb1 Five\plain\tab\fs14\f3\cf0\cb1 Six" +
				@"\plain\tab\fs14\f3\cf0\cb1 Seven {\fs18\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Eight{\fs15\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo("\tOne\tTwo\tThree\tFour\tFive\tSix\tSeven \n\t\t\t\t\t\t\tEight\n"));
		}

		[Test]
		public void MultipleLinesWithTabs()
		{
			File.WriteAllText(m_TempFile, @"{\rtf1\ansi\ansicpg65001\deff0\deflang1033{" +
				@"\fonttbl{\f0\fnil Arial;}}{\colortbl\red0\green0\blue0 ;}" +
				@"{\*\generator NRtfTree Library 0.3.0;}\uc1\margl1133\margr1133\margt1133" +
				@"\margb1133\viewkind4" +
				@"\pard\plain\sb82\tx1362\tqr\tx2785\tqr\tx3334\tqr\tx4220\tx4310\tx4466\tx5443" +
				@"{\plain\tab\fs14\f3\cf0\cb1 One\plain\tab\fs14\f3\cf0\cb1 Two\plain\tab\fs14\f3\cf0\cb1 Three" +
				@"\plain\tab\fs14\f3\cf0\cb1 Four\plain\tab\fs14\f3\cf0\cb1 Five\plain\tab\fs14\f3\cf0\cb1 Six" +
				@"\plain\tab\fs14\f3\cf0\cb1 Seven {\fs18\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Eight{\fs15\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Nine{\fs15\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo("\tOne\tTwo\tThree\tFour\tFive\tSix\tSeven \n\t\t\t\t\t\t\tEight\n\t\t\t\t\t\t\tNine\n"));
		}

		[Test]
		public void WithHeading()
		{
			File.WriteAllText(m_TempFile, @"{\rtf1\ansi\ansicpg65001\deff0\deflang1033{" +
				@"\fonttbl{\f0\fnil Arial;}}{\colortbl\red0\green0\blue0 ;}" +
				@"{\*\generator NRtfTree Library 0.3.0;}\uc1\margl1133\margr1133\margt1133" +
				@"\margb1133\viewkind4" +
				@"\pard\plain\sb263\tqr\tx802\tx907\tqr\tx10263{\plain\tab\fs18\f8\cf0\cb1 3224" +
				@"\plain\tab\fs18\b\f4\cf0\cb1 Weitergel. Spenden v.a. WO's\plain\tab\fs18\f8\cf0\cb1 319,82{\fs22\par}}" +
				@"\pard\plain\sb82\tx1362\tqr\tx2785\tqr\tx3334\tqr\tx4220\tx4310\tx4466\tx5443" +
				@"{\plain\tab\fs14\f3\cf0\cb1 One\plain\tab\fs14\f3\cf0\cb1 Two\plain\tab\fs14\f3\cf0\cb1 Three" +
				@"\plain\tab\fs14\f3\cf0\cb1 Four\plain\tab\fs14\f3\cf0\cb1 Five\plain\tab\fs14\f3\cf0\cb1 Six" +
				@"\plain\tab\fs14\f3\cf0\cb1 Seven {\fs18\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Eight{\fs15\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Nine{\fs15\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo("\t3224\tWeitergel. Spenden v.a. WO's\t319,82\n" +
				"\tOne\tTwo\tThree\tFour\tFive\tSix\tSeven \n" +
				"\t\t\t\t\t\t\tEight\n\t\t\t\t\t\t\tNine\n"));
		}

		[Test]
		public void WithHeadingAfter()
		{
			File.WriteAllText(m_TempFile, @"{\rtf1\ansi\ansicpg65001\deff0\deflang1033{" +
				@"\fonttbl{\f0\fnil Arial;}}{\colortbl\red0\green0\blue0 ;}" +
				@"{\*\generator NRtfTree Library 0.3.0;}\uc1\margl1133\margr1133\margt1133" +
				@"\margb1133\viewkind4" +
				@"\pard\plain\sb263\tqr\tx802\tx907\tqr\tx10263{\plain\tab\fs18\f8\cf0\cb1 3224" +
				@"\plain\tab\fs18\b\f4\cf0\cb1 Weitergel. Spenden v.a. WO's\plain\tab\fs18\f8\cf0\cb1 319,82{\fs22\par}}" +
				@"\pard\plain\sb82\tx1362\tqr\tx2785\tqr\tx3334\tqr\tx4220\tx4310\tx4466\tx5443" +
				@"{\plain\tab\fs14\f3\cf0\cb1 One\plain\tab\fs14\f3\cf0\cb1 Two\plain\tab\fs14\f3\cf0\cb1 Three" +
				@"\plain\tab\fs14\f3\cf0\cb1 Four\plain\tab\fs14\f3\cf0\cb1 Five\plain\tab\fs14\f3\cf0\cb1 Six" +
				@"\plain\tab\fs14\f3\cf0\cb1 Seven {\fs18\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Eight{\fs15\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Nine{\fs15\par}}" +
				@"\pard\plain\sb263\tqr\tx802\tx907\tqr\tx10263{\plain\tab\fs18\f8\cf0\cb1 3225" +
				@"\plain\tab\fs18\b\f4\cf0\cb1 Sonstwas\plain\tab\fs18\f8\cf0\cb1 111,22{\fs22\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo("\t3224\tWeitergel. Spenden v.a. WO's\t319,82\n" +
				"\tOne\tTwo\tThree\tFour\tFive\tSix\tSeven \n" +
				"\t\t\t\t\t\t\tEight\n\t\t\t\t\t\t\tNine\n" +
				"\t3225\tSonstwas\t111,22\n"));
		}

		[Test]
		public void AddressInexactMatchingTabs()
		{
			File.WriteAllText(m_TempFile, @"{\rtf1\ansi\ansicpg65001\deff0\deflang1033{" +
				@"\fonttbl{\f0\fnil Arial;}}{\colortbl\red0\green0\blue0 ;}" +
				@"{\*\generator NRtfTree Library 0.3.0;}\uc1\margl1133\margr1133\margt1133" +
				@"\margb1133\viewkind4" +
				@"\pard\plain\sb263\tqr\tx802\tx907\tqr\tx10263{\plain\tab\fs18\f8\cf0\cb1 3224" +
				@"\plain\tab\fs18\b\f4\cf0\cb1 Weitergel. Spenden v.a. WO's\plain\tab\fs18\f8\cf0\cb1 319,82{\fs22\par}}" +
				@"\pard\plain\sb56\tqr\tx624\tx714\tx3174\tx5794\tqr\tx8386\tqr\tx9296" +
				@"{\plain\tab\fs14\f3\cf0\cb1 12345\plain\tab\fs14\f3\cf0\cb1 Organization" +
				@"\plain\tab\fs14\f3\cf0\cb1 Street 1" +
				@"\plain\tab\fs14\f3\cf0\cb1 p: 0123456\plain\tab\fs14\f3\cf0\cb1 1\plain\tab\fs14\f3\cf0\cb1 150,00{\fs18\par}}" +
				@"\pard\plain\sb22\tx690\tx3174{\plain\tab\fs14\f3\cf0\cb1 OrgCont" +
				@"\plain\tab\fs14\f3\cf0\cb1 12345 Somewhere{\fs18\par}}" +
				@"\pard\plain\sb10\tx5782{\plain\tab\fs14\f3\cf0\cb1 someone@email.com{\fs18\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo("\t3224\tWeitergel. Spenden v.a. WO's\t319,82\n" +
				"\t12345\tOrganization\tStreet 1\tp: 0123456\t1\t150,00\n" +
				"\t\tOrgCont\t12345 Somewhere\n" +
				"\t\t\t\tsomeone@email.com\n"));
		}
	}
}

