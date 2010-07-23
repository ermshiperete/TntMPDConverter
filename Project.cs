using System;
using System.Text.RegularExpressions;

namespace TntMPDConverter
{
	public class Project: State
	{
		public Project(Scanner reader)
			: base(reader)
        {
        }

        public override State NextState()
        {
        	ProcessProjectNo();
            for (var line = Reader.ReadLine(); !Reader.EndOfStream; line = Reader.ReadLine())
            {
                if (Account.IsAccount(line))
                {
                    Reader.UnreadLine(line);
                    break;
                }
            }
            if (Reader.EndOfStream)
                return new End(Reader);

			return new Account(Reader);
        }

		public static bool IsProject(string line)
		{
			var regex = new Regex("Projekt\t[0-9]+  [^\t]+");
			return regex.IsMatch(line);
		}

		public int ProjectNo { get; private set; }

		protected void ProcessProjectNo()
		{
			var line = Reader.ReadLine();
			var regex = new Regex("Projekt\t(?<no>[0-9]+)");
			ProjectNo = Convert.ToInt32(regex.Match(line).Groups["no"].Value);
		}
	}
}
