// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Text;
using Net.Sgoliver.NRtfTree.Core;

namespace TntMPDConverter
{
	public class RtfProcessLine
	{
		private StringBuilder m_Builder;
		protected RtfLine m_CurrentLine;
		protected RtfLine m_PrevLine;
		protected RtfLine m_CurrentPara;

		public RtfProcessLine()
		{
			m_CurrentLine = new RtfLine();
			m_PrevLine = new RtfLine();
			m_CurrentLine = new RtfLine();
		}

		private void AddTabChar()
		{
			m_Builder.Append('\t');
		}

		public void AddText(StringBuilder builder, string text)
		{
			if (!string.IsNullOrEmpty(text))
				builder.Append(text);
		}

		private bool TabStopsAreEqual(int tabStop1, int tabStop2)
		{
			// check for inexact matching tabs +/- 5%
			return tabStop1 >= tabStop2 * 0.95 && tabStop1 <= tabStop2 * 1.05;
		}

		private int MissingTabStops
		{
			get
			{
				if (m_CurrentPara.TabStops == null || m_CurrentLine.TabStops.Count < 1)
					return 0;

				for (int i = 0; i < m_CurrentPara.TabStops.Count; i++)
				{
					if (TabStopsAreEqual(m_CurrentLine.TabStops[0], m_CurrentPara.TabStops[i]))
						return i;
				}
				return 0;
			}
		}

		protected bool IsSameParaKindAsPrevious
		{
			get
			{
				if (m_CurrentPara == null)
					return false;

				// check if we have (almost) the same tabstops as in the previous para. If so
				// it's not a continuation line.
				for (int i = 0, j = 0; i < m_CurrentLine.TabStops.Count; i++, j++)
				{
					if (i > 0)
					{
						for (; j < m_CurrentPara.TabStops.Count && j < i + 2; j++)
						{
							if (TabStopsAreEqual(m_CurrentLine.TabStops[i], m_CurrentPara.TabStops[j]))
								break;
						}
					}
					if (j >= m_CurrentPara.TabStops.Count ||
						!TabStopsAreEqual(m_CurrentLine.TabStops[i], m_CurrentPara.TabStops[j]))
						return false;
				}
				return true;
			}
		}

		protected bool IsContinuationLine
		{
			get
			{
				if (m_CurrentPara == null)
					return false;
				if (m_CurrentPara.TabStops.Count < m_CurrentLine.TabStops.Count)
					return false;
				if (m_CurrentPara.IsHeader != m_CurrentLine.IsHeader)
					return false;

				if (IsSameParaKindAsPrevious)
					return false;

				int j = 0;
				bool foundTabstop = false;
				for (int i = 0; i < m_CurrentLine.TabStops.Count; i++)
				{
					foundTabstop = false;
					for (; j < m_CurrentPara.TabStops.Count; j++)
					{
						if (TabStopsAreEqual(m_CurrentLine.TabStops[i], m_CurrentPara.TabStops[j]))
						{
							foundTabstop = true;
							break;
						}
					}
				}
				return foundTabstop;
			}
		}

		private void CreateOrContinuePara()
		{
			if (IsContinuationLine || IsSameParaKindAsPrevious)
				return;

			if (m_PrevLine.IsHeader == m_CurrentLine.IsHeader)
			{
				m_CurrentPara = new RtfLine(m_PrevLine);
				m_PrevLine = new RtfLine();
			}
			else
			{
				m_CurrentPara = new RtfLine(m_CurrentLine);
				m_PrevLine = new RtfLine();
			}
		}

		public void ProcessGroup(StringBuilder builder, RtfTreeNode node, RtfLine currentLine)
		{
			m_Builder = builder;
			m_PrevLine = m_CurrentLine;
			m_CurrentLine = currentLine;
			CreateOrContinuePara();

			var prevBuilder = m_Builder;
			m_Builder = new StringBuilder();
			int tabCount = 0;
			foreach (RtfTreeNode child in node.ChildNodes)
			{
				switch (child.NodeType)
				{
					case RtfNodeType.Group:
					case RtfNodeType.Text:
					case RtfNodeType.Control:
						AddText(m_Builder, child.Text);
						break;
					case RtfNodeType.Keyword:
						{
							switch(child.NodeKey)
							{
								case "tab":
									AddTabChar();
									tabCount++;
									break;
								case "b":
									m_CurrentLine.IsHeader = true;
									break;
								case "par":
									m_Builder.AppendLine();
									break;
							}
							break;
						}
				}
			}
			var newBuilder = m_Builder;
			m_Builder = prevBuilder;
			if (IsContinuationLine)
			{
				var tabsToAdd = MissingTabStops;
				for (int i = 0; i < tabsToAdd; i++)
					AddTabChar();
			}
			m_Builder.Append(newBuilder.ToString());
		}

	}
}

