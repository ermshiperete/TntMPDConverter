// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System.IO;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class RtfConverterTests
	{
		[Test]
		public void SimpleConversion()
		{
			var tempFile = Path.GetTempFileName();
			File.WriteAllText(tempFile, "{\\rtf1\\ansi\\ansicpg65001\\deff0\\deflang1033{" +
				"\\fonttbl{\\f0\\fnil Arial;}}{\\colortbl\\red0\\green0\\blue0 ;}" +
				"{\\*\\generator NRtfTree Library 0.3.0;}\\uc1\\margl1133\\margr1133\\margt1133" +
				"\\margb1133\\viewkind4\\pard\\cf0\\fs40\\f0\\b\\ul First line\\line\\par Second line\\par}");

			var converter = new RtfConverter(tempFile);
			Assert.That(converter.Text, Is.EqualTo("First line\n\nSecond line\n"));
		}
	}
}

