using System;
using System.Collections.Generic;
using System.IO;

namespace TntMPDConverter
{
	public class MyReplacementManager: ReplacementManager
	{
		public static string OriginalReplacementFileName { get; private set;}
		public MyReplacementManager(string fileName)
		{
			if (OriginalReplacementFileName == null)
				OriginalReplacementFileName = ReplacementFileName;
			ReplacementFileName = fileName;
		}

		#region Static methods
		public static MyReplacementManager Instance { get; private set;}

		public static void Create()
		{
			Instance = new MyReplacementManager(Path.GetTempFileName());
		}

		public static void Finish()
		{
			File.Delete(Instance.ReplacementFileNameForTests);
			Instance = null;
		}

		public static void CreateReplacementFile(string content)
		{
			using (var writer = new StreamWriter(Instance.ReplacementFileNameForTests))
			{
				writer.WriteLine(content);
			}
			Instance.ReReadReplacementFile();
		}
		#endregion

		public string ReplacementFileNameForTests
		{
			get { return ReplacementFileName;}
			set { ReplacementFileName = value; }
		}

		public Dictionary<int, Dictionary<string, Replacement>> GetReplacementInfo()
		{
			return ReplacementInfo;
		}

		public Dictionary<int, List<string>> GetIncludeInfo()
		{
			return IncludeInfo;
		}

		public Dictionary<int, List<string>> GetExcludeInfo()
		{
			return ExcludeInfo;
		}

		public void ReReadReplacementFile()
		{
			UpdateReplacementInfo();
		}
	}
}

