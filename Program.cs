// Copyright (c) 2013, Eberhard Beilharz
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Windows.Forms;
using System.IO;
using TntMPDConverter.Properties;

namespace TntMPDConverter
{
	static internal class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			if (args.Length < 1 || args[0] != "--nested")
			{
				Upgrade();

				// we need to use different config file depending on OS.
				// On Linux we want to use SilUtils.dll compiled with GTK so that we get GTK file
				// dialogs whereas on Windows we need a different SilUtils.dll so that we don't have
				// the dependency on GTK. See http://stackoverflow.com/a/305711 for algorithm.
				var setup = new AppDomainSetup();
				setup.ApplicationBase = "file://" + Environment.CurrentDirectory;
				setup.DisallowBindingRedirects = true;
				setup.DisallowCodeDownload = true;

				if (Environment.OSVersion.Platform == PlatformID.Unix)
					setup.ConfigurationFile = Path.Combine(Environment.CurrentDirectory, "linux.config");
				else
					setup.ConfigurationFile = Path.Combine(Environment.CurrentDirectory, "windows.config");

				var domain = AppDomain.CreateDomain("TntMPDConvert", null, setup);
				var newArgs = new string[args.Length + 1];
				newArgs[0] = "--nested";
				Array.Copy(args, 0, newArgs, 1, args.Length);
				domain.ExecuteAssembly("TntMPDConverter.exe", newArgs);
			}
			else
			{
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

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				new Form1().ShowDialog();
			}
		}

		private static void Upgrade()
		{
			if (!Settings.Default.UpgradeDone)
			{
				Settings.Default.Upgrade();
				Settings.Default.UpgradeDone = true;
				Settings.Default.Save();
			}
		}
	}
}
