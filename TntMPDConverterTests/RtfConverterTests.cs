// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

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
			Assert.That(converter.Text, Is.EqualTo(
@"First line

Second line
"));
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
			Assert.That(converter.Text, Is.EqualTo(
@"	First	Line
"));
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
				@"\plain\tab\fs14\f3\cf0\cb1 Seven{\fs18\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Eight{\fs15\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo(
@"	One	Two	Three	Four	Five	Six	Seven
							Eight
"));
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
				@"\plain\tab\fs14\f3\cf0\cb1 Seven{\fs18\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Eight{\fs15\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Nine{\fs15\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo(
@"	One	Two	Three	Four	Five	Six	Seven
							Eight
							Nine
"));
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
				@"\plain\tab\fs14\f3\cf0\cb1 Seven{\fs18\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Eight{\fs15\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Nine{\fs15\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo(
@"	3224	Weitergel. Spenden v.a. WO's	319,82
	One	Two	Three	Four	Five	Six	Seven
							Eight
							Nine
"));
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
				@"\plain\tab\fs14\f3\cf0\cb1 Seven{\fs18\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Eight{\fs15\par}}" +
				@"\pard\plain\tx5443{\plain\tab\fs14\f3\cf0\cb1 Nine{\fs15\par}}" +
				@"\pard\plain\sb263\tqr\tx802\tx907\tqr\tx10263{\plain\tab\fs18\f8\cf0\cb1 3225" +
				@"\plain\tab\fs18\b\f4\cf0\cb1 Sonstwas\plain\tab\fs18\f8\cf0\cb1 111,22{\fs22\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo(
@"	3224	Weitergel. Spenden v.a. WO's	319,82
	One	Two	Three	Four	Five	Six	Seven
							Eight
							Nine
	3225	Sonstwas	111,22
"));
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
			Assert.That(converter.Text, Is.EqualTo(
@"	3224	Weitergel. Spenden v.a. WO's	319,82
	12345	Organization	Street 1	p: 0123456	1	150,00
		OrgCont	12345 Somewhere
				someone@email.com
"));
		}

		[Test]
		public void MultipleAddresses()
		{
			File.WriteAllText(m_TempFile, @"{\rtf1\ansi\ansicpg65001\deff0\deflang1033{" +
				@"\fonttbl{\f0\fnil Arial;}}{\colortbl\red0\green0\blue0 ;}" +
				@"{\*\generator NRtfTree Library 0.3.0;}\uc1\margl1133\margr1133\margt1133" +
				@"\margb1133\viewkind4" +
				@"\pard\plain\sb97\tqr\tx624\tx714\tx3174\tx5794\tqr\tx8386\tqr\tx9296" +
				@"{\plain\tab\fs14\f3\cf0\cb1 31953\plain\tab\fs14\f3\cf0\cb1 APerson, A"+
				@"\plain\tab\fs14\f3\cf0\cb1 Street 5\plain\tab\fs14\f3\cf0\cb1 p: 00972/12345" +
				@"\plain\tab\fs14\f3\cf0\cb1 1\plain\tab\fs14\f3\cf0\cb1 40,00{\fs18\par}}" +
				@"\pard\plain\sb22\tx3174{\plain\tab\fs14\f3\cf0\cb1 72270 Somewhere{\fs18\par}}" +
				@"\pard\plain\sb83\tqr\tx624\tx714\tx3174\tqr\tx8386\tqr\tx9296" +
				@"{\plain\tab\fs14\f3\cf0\cb1 40462\plain\tab\fs14\f3\cf0\cb1 BPerson, B" +
				@"\plain\tab\fs14\f3\cf0\cb1 Road 43" +
				@"\plain\tab\fs14\f3\cf0\cb1 1\plain\tab\fs14\f3\cf0\cb1 30,00{\fs18\par}}" +
				@"\pard\plain\sb22\tx3174{\plain\tab\fs14\f3\cf0\cb1 71083 Otherwhere{\fs18\par}}" +
				@"\pard\plain\sb83\tqr\tx624\tx714\tx3174\tqr\tx8386\tqr\tx9296" +
				@"{\plain\tab\fs14\f3\cf0\cb1 40883\plain\tab\fs14\f3\cf0\cb1 CPerson, C" +
				@"\plain\tab\fs14\f3\cf0\cb1 Alley 32" +
				@"\plain\tab\fs14\f3\cf0\cb1 3\plain\tab\fs14\f3\cf0\cb1 -60,00{\fs18\par}}" +
				@"\pard\plain\sb22\tx3174{\plain\tab\fs14\f3\cf0\cb1 70734 Nowhere{\fs18\par}}" +
				@"\pard\plain\sb83\tqr\tx624\tx714\tx3174\tqr\tx8386\tqr\tx9296" +
				@"{\plain\tab\fs14\f3\cf0\cb1 28219\plain\tab\fs14\f3\cf0\cb1 DPerson, D" +
				@"\plain\tab\fs14\f3\cf0\cb1 Main St. 73" +
				@"\plain\tab\fs14\f3\cf0\cb1 1\plain\tab\fs14\f3\cf0\cb1 12,00{\fs18\par}}" +
				@"\pard\plain\sb22\tx3174{\plain\tab\fs14\f3\cf0\cb1 12345 Berlin{\fs18\par}}" +
				@"\pard\plain\sb10\tx5782{\plain\tab\fs14\f3\cf0\cb1 my.email@example.de{\fs18\par}}" +
				@"}");

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo(
@"	31953	APerson, A	Street 5	p: 00972/12345	1	40,00
			72270 Somewhere
	40462	BPerson, B	Road 43	1	30,00
			71083 Otherwhere
	40883	CPerson, C	Alley 32	3	-60,00
			70734 Nowhere
	28219	DPerson, D	Main St. 73	1	12,00
			12345 Berlin
				my.email@example.de
"));
		}

		[Test]
		public void AddressHeaderFiltered()
		{
			File.WriteAllText(m_TempFile, @"{\rtf1\ansi\ansicpg65001\deff0\deflang1033{" +
				@"\fonttbl{\f0\fnil Arial;}}{\colortbl\red0\green0\blue0 ;}" +
				@"{\*\generator NRtfTree Library 0.3.0;}\uc1\margl1133\margr1133\margt1133" +
				@"\margb1133\viewkind4" +
				@"\pard\plain\sb524\tx90{\plain\fs20\b\f2\cf2\cb1 Spenderübersicht{\fs25\par}}" +
				@"\pard\plain\sb75\tx90\tx5782\tqc\tx8845{\plain\fs14\b\f1\cf0\cb1 Spender-" +
				@"\plain\tab\fs14\b\f1\cf0\cb1 Telefon (privat, dienstl.)\plain\tab\fs14\b\f1\cf0\cb1 Spenden{\fs18\par}}" +
				@"\pard\plain\sb63\tx90\tx690\tx3174\tx5782\tx8050\tqr\tx9303{\plain\fs14\b\f1\cf0\cb1 Nr." +
				@"\plain\tab\fs14\b\f1\cf0\cb1 Name\plain\tab\fs14\b\f1\cf0\cb1 Adresse" +
				@"\plain\tab\fs14\b\f1\cf0\cb1 Fax, E-Mail\plain\tab\fs14\b\f1\cf0\cb1 Anz." +
				@"\plain\tab\fs14\b\f1\cf0\cb1 \u8364\'80{\fs18\par}}" +
				@"\pard\plain\sb97\tqr\tx624\tx714\tx3174\tx5794\tqr\tx8386\tqr\tx9296" +
				@"{\plain\tab\fs14\f3\cf0\cb1 31953\plain\tab\fs14\f3\cf0\cb1 APerson, A"+
				@"\plain\tab\fs14\f3\cf0\cb1 Street 5\plain\tab\fs14\f3\cf0\cb1 p: 00972/12345" +
				@"\plain\tab\fs14\f3\cf0\cb1 1\plain\tab\fs14\f3\cf0\cb1 40,00{\fs18\par}}" +
				@"\pard\plain\sb22\tx3174{\plain\tab\fs14\f3\cf0\cb1 72270 Somewhere{\fs18\par}}" +
				@"}", Encoding.GetEncoding("Windows-1252"));

			var converter = new RtfConverter(m_TempFile);
			Assert.That(converter.Text, Is.EqualTo(@"Spenderübersicht
	31953	APerson, A	Street 5	p: 00972/12345	1	40,00
			72270 Somewhere
"));
		}

	}
}

