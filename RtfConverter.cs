// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System;
using System.Text;
using Net.Sgoliver.NRtfTree.Core;

namespace TntMPDConverter
{
	public class RtfConverter
	{
		private string m_FileName;
		protected RtfLine m_CurrentLine;
		private StringBuilder m_Builder;
		private bool IsParaOpen { get; set; }
		private bool EncounteredProject { get; set; }

		public RtfConverter(string fileName)
		{
			m_FileName = fileName;
			m_CurrentLine = new RtfLine();
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
			else if (text.StartsWith("Spender-\tTelefon (privat, dienstl.)\tSpenden") ||
				text.StartsWith("Nr.\tName\tAdresse\tFax, E-Mail\tAnz."))
			{
				return false;
			}
			return !string.IsNullOrEmpty(text);
		}

		private void StartNewPara()
		{
			m_CurrentLine = new RtfLine();
			IsParaOpen = true;
		}

		private void AddTabStop(int value)
		{
			m_CurrentLine.TabStops.Add(value);
		}

		private string Convert()
		{
			var processLine = new RtfProcessLine();
			var tree = new RtfTree();
			tree.LoadRtfFile(m_FileName, Encoding.GetEncoding("Windows-1252"));

			m_Builder = new StringBuilder();
			foreach (RtfTreeNode node in tree.MainGroup.ChildNodes)
			{
				switch (node.NodeType)
				{
					case RtfNodeType.Group:
						if (IncludeText(node.Text) && IsParaOpen)
							processLine.ProcessGroup(m_Builder, node, m_CurrentLine);
						break;
					case RtfNodeType.Text:
						if (IncludeText(node.Text) && IsParaOpen)
							processLine.AddText(m_Builder, node.Text);
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

	}
}

