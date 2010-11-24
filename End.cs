namespace TntMPDConverter
{
	using System;

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

