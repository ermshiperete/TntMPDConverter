// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System;
using System.Collections.Generic;

namespace TntMPDConverter
{
	public class RtfLine
	{
		public RtfLine()
		{
			TabStops = new List<int>();
		}

		public RtfLine(RtfLine otherLine): this()
		{
			if (otherLine != null)
			{
				IsHeader = otherLine.IsHeader;
				TabStops.AddRange(otherLine.TabStops);
			}
		}

		public List<int> TabStops;
		public bool IsHeader;
	}

}

