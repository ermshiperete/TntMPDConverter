using System;
using Xwt;
using System.IO;
using TntMPDConverter.Properties;

namespace TntMPDConverter
{
	static internal class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			var tkType = ToolkitType.Wpf;
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				tkType = ToolkitType.Gtk;
			Application.Initialize(tkType);
			try
			{
				Upgrade();

				if (args.Length == 2)
				{
					if (!File.Exists(args[0]))
					{
						Console.WriteLine("Source file {0} doesn't exist.", args[0]);
						return;
					}
					if (!Directory.Exists(args[1]))
					{
						Console.WriteLine("Target directory {0} doesn't exist.", args[1]);
						return;
					}
					Settings.Default.SourceFile = args[0];
					Settings.Default.TargetPath = args[1];
					new ConvertStatement().DoConversion();
					Settings.Default.Save();
					return;
				}

				using (var dlg = new ConvertDialog())
				{
					dlg.Run();
				}
			}
			finally
			{
				Application.Dispose();
			}
		}

		private static void Upgrade()
		{
			if (Settings.Default.UpgradeDone)
				return;

			Settings.Default.Upgrade();
			Settings.Default.UpgradeDone = true;
			Settings.Default.Save();
		}
	}
}
