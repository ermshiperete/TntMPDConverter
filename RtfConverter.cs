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

		public RtfConverter(string fileName)
		{
			m_FileName = fileName;
		}

		public string Text { get { return Convert(); }}

		private string Convert()
		{
			var tree = new RtfTree();
			tree.LoadRtfFile(m_FileName, Encoding.GetEncoding("Windows-1252"));

			return tree.Text;
		}
	}
}

