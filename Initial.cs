// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
namespace TntMPDConverter
{

	public class Initial : State
	{
		public Initial(Scanner reader) : base(reader)
		{
		}

		public override State NextState()
		{
			for (var line = Reader.ReadLine(); !Reader.EndOfStream; line = Reader.ReadLine())
			{
				if (Project.IsProject(line))
				{
					Reader.UnreadLine(line);
					break;
				}
			}
			if (Reader.EndOfStream)
				return new End(Reader);

			return new Project(Reader);
		}

	}
}

