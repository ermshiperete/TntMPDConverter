using System;
using System.Windows.Forms;
using System.IO;
using TntMPDConverter.Properties;

namespace TntMPDConverter
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
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
        	
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new Form1().ShowDialog();
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