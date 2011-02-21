using System.IO;

namespace TntMPDConverter
{
	public class State
	{
		internal bool IsValid
		{
			get;
			set;
		}
		internal Scanner Reader
		{
			get;
			set;
		}

		public State(Scanner reader)
		{
			IsValid = true;
			Reader = reader;
		}

		protected State CheckForStartOfNewState(string line)
		{
			if (Account.IsAccount(line))
			{
				Reader.UnreadLine(line);
				return new Account(Reader);
			}
			if (Reader.EndOfStream)
			{
				return new End(Reader);
			}
			string[] textArray1 = line.Split(new[] { '\t' });
			if (textArray1.Length > 1)
			{
				if (textArray1[0] == "Projekt:")
				{
					return null;
				}
			}
			else if (ProcessDonors.IsDonors(line))
			{
				Reader.UnreadLine(line);
				return new ProcessDonors(Reader);
			}
			return null;
		}

		public static State Initialize(TextReader reader)
		{
			return new Initial(new Scanner(reader));
		}

		public virtual State NextState()
		{
			return null;
		}

		public virtual bool EndOfStream
		{
			get { return false; }
		}

		public bool StateValid
		{
			get { return IsValid; }
		}
	}
}

