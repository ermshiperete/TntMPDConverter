// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System;
using System.Text;
using Net.Sgoliver.NRtfTree.Core;
using System.Collections.Generic;

namespace TntMPDConverter
{
	public class RtfConverter
	{
		private class Line
		{
			public Line()
			{
				TabStops = new List<int>();
			}

			public List<int> TabStops;
			public bool IsHeader;
		}

		private string m_FileName;
		private Line m_CurrentLine;
		private Line m_PrevLine;
		private Line m_CurrentPara;
		private StringBuilder m_Builder;
		private bool IsParaOpen { get; set; }
		private bool EncounteredProject { get; set; }

		public RtfConverter(string fileName)
		{
			m_FileName = fileName;
			m_CurrentLine = new Line();
		}

		public string Text { get { return Convert(); }}

		private bool IncludeText(string text)
		{
			if (text.StartsWith("Projekt:"))
			{
				if (EncounteredProject)
					return false;
				EncounteredProject = true;
			}
			return !string.IsNullOrEmpty(text);
		}

		private string Convert()
		{
			var tree = new RtfTree();
			tree.LoadRtfFile(m_FileName, Encoding.GetEncoding("Windows-1252"));

			m_Builder = new StringBuilder();
			foreach (RtfTreeNode node in tree.MainGroup.ChildNodes)
			{
				switch (node.NodeType)
				{
					case RtfNodeType.Group:
						if (IncludeText(node.Text) && IsParaOpen)
							ProcessGroup(node);
						break;
					case RtfNodeType.Text:
						if (IncludeText(node.Text) && IsParaOpen)
							AddText(node.Text);
						break;
					case RtfNodeType.Keyword:
					{
						switch (node.NodeKey)
						{
							case "line":
							case "par":
								if (IsParaOpen)
									m_Builder.AppendLine();
								break;
							case "pard":
								StartNewPara();
								break;
							case "tx":
								AddTabStop(node.Parameter);
								break;
						}
						break;
					}
				}
			}
			Console.WriteLine("Document: " + m_Builder.ToString());
			return m_Builder.ToString();
		}

		private void StartNewPara()
		{
			m_PrevLine = m_CurrentLine;
			m_CurrentLine = new Line();
			IsParaOpen = true;
		}

		private void AddTabStop(int value)
		{
			m_CurrentLine.TabStops.Add(value);
		}

		private void AddTabChar()
		{
			m_Builder.Append('\t');
		}

		private void AddText(string text)
		{
			if (!string.IsNullOrEmpty(text))
				m_Builder.Append(text);
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

		private bool IsContinuationLine
		{
			get
			{
				if (m_CurrentPara == null)
					return false;
				if (m_CurrentPara.TabStops.Count < m_CurrentLine.TabStops.Count)
					return false;
				if (m_CurrentPara.IsHeader != m_CurrentLine.IsHeader)
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

		private void ProcessGroup(RtfTreeNode node)
		{
			if (!IsContinuationLine)
			{
				m_CurrentPara = new Line();
				m_CurrentPara.TabStops.AddRange(m_PrevLine.TabStops);
				m_PrevLine.TabStops = new List<int>();
			}

			var prevBuilder = m_Builder;
			m_Builder = new StringBuilder();
			int tabCount = 0;
			foreach (RtfTreeNode child in node.ChildNodes)
			{
				switch (child.NodeType)
				{
					case RtfNodeType.Group:
					case RtfNodeType.Text:
						AddText(child.Text);
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

