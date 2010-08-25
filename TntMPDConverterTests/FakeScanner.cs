using TntMPDConverter;

namespace TntMPDConverterTests
{
	public class FakeScanner: Scanner
	{
		private readonly string[] m_Lines;
		private int m_Index;
		public FakeScanner(string lines): base(null)
		{
			m_Lines = lines.Split('\n');
		}

		public override string ReadLine()
		{
			if (m_line != null)
			{
				var line = m_line;
				m_line = null;
				return line;
			}
			if (m_Index >= m_Lines.Length)
				return null;
			return m_Lines[m_Index++].TrimEnd('\r');
		}

		public override bool EndOfStream
		{
			get
			{
				return m_Index >= m_Lines.Length;
			}
		}
	}
}
