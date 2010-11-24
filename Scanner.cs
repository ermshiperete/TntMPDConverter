using System.Diagnostics;
using System.IO;

namespace TntMPDConverter
{
	public class Scanner
	{
		protected string m_line;
		private readonly TextReader m_reader;

		public Scanner(TextReader reader)
		{
			m_reader = reader;
		}

		public virtual string ReadLine()
		{
			if (m_line != null)
			{
				var line = m_line;
				m_line = null;
				return line;
			}
			return m_reader.ReadLine();
		}

		public void UnreadLine(string line)
		{
			m_line = line;
		}

		public virtual bool EndOfStream
		{
			get
			{
				if (m_reader is StreamReader)
					return ((StreamReader)m_reader).EndOfStream;
				return m_reader.Peek() == -1;
			}
		}
	}
}

