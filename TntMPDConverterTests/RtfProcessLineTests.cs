// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using NUnit.Framework;

namespace TntMPDConverter
{
	[TestFixture]
	public class RtfProcessLineTests
	{
		private class RtfConverterDouble: RtfProcessLine
		{
			public RtfConverterDouble(int[] currentLineTabStops,
				int[] prevLineTabStops, int[] currentParaTabStops)
			{
				m_CurrentLine = new RtfLine();
				if (currentLineTabStops != null)
					m_CurrentLine.TabStops.AddRange(currentLineTabStops);
				m_PrevLine = new RtfLine();
				if (prevLineTabStops != null)
					m_PrevLine.TabStops.AddRange(prevLineTabStops);
				if (currentParaTabStops != null)
				{
					m_CurrentPara = new RtfLine();
					m_CurrentPara.TabStops.AddRange(currentParaTabStops);
				}
			}

			public bool CallIsContinuationLine { get { return IsContinuationLine; }}
			public bool CallIsSameParaKindAsPrevious { get { return IsSameParaKindAsPrevious; }}
		}

		[Test]
		[TestCase(new[] { 30}, null, new [] {10, 20, 30, 40, 50}, /* expected */ true, TestName="SecondLine")]
		[TestCase(new[] { 10, 20, 30, 50}, new[] { 30 }, new [] {10, 20, 30, 40, 50}, /* expected */ false, TestName="NewParaWithSligthlyDifferentTabs")]
		[TestCase(new[] { 20, 30, 50}, null, new [] {10, 44}, /* expected */ false, TestName="ParaAfterSectionHead")]
		[TestCase(new[] { 10}, null, new[] { 20, 30, 40}, /* expected */ false, TestName="SectionHeadAfterPara")]
		public void IsContinuationLine(int[] currentLineTabs, int[] prevLineTabs,
			int[] currentParTabs, bool expectedResult)
		{
			var rtfConverter = new RtfConverterDouble(currentLineTabs, prevLineTabs, currentParTabs);

			Assert.That(rtfConverter.CallIsContinuationLine, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(new[] { 30}, null, new [] {10, 20, 30, 40, 50}, /* expected */ false, TestName="SecondLine")]
		[TestCase(new[] { 10, 20, 30, 50}, new[] { 30 }, new [] {10, 20, 30, 40, 50}, /* expected */ true, TestName="NewParaWithSligthlyDifferentTabs")]
		[TestCase(new[] { 20, 30, 50}, null, new [] {10, 44}, /* expected */ false, TestName="ParaAfterSectionHead")]
		[TestCase(new[] { 10}, null, new[] { 20, 30, 40}, /* expected */ false, TestName="SectionHeadAfterPara")]
		public void IsSameParaKindAsPrevious(int[] currentLineTabs, int[] prevLineTabs,
			int[] currentParTabs, bool expectedResult)
		{
			var rtfConverter = new RtfConverterDouble(currentLineTabs, prevLineTabs, currentParTabs);

			Assert.That(rtfConverter.CallIsSameParaKindAsPrevious, Is.EqualTo(expectedResult));
		}
			}
}

