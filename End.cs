// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
namespace TntMPDConverter
{
	internal class End : State
	{
		public End(Scanner reader) : base(reader)
		{
		}

		public override bool EndOfStream
		{
			get { return true; }
		}

	}
}

