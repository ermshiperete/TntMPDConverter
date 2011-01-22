namespace TntMPDConverter.Properties
{
	using System;
	using System.CodeDom.Compiler;
	using System.Configuration;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;

	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0"), CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		static Settings()
		{
			Settings.defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
		}


		[DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("false")]
		public bool UpgradeDone
		{
			get { return (bool)this["UpgradeDone"]; }
			set { this["UpgradeDone"] = value; }
		}

		public static Settings Default
		{
			get { return Settings.defaultInstance; }
		}

		[DefaultSettingValue(""), DebuggerNonUserCode, UserScopedSetting]
		public string SourceFile
		{
			get { return (string)this["SourceFile"]; }
			set { this["SourceFile"] = value; }
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string TargetPath
		{
			get { return (string)this["TargetPath"]; }
			set { this["TargetPath"] = value; }
		}


		private static Settings defaultInstance;
	}
}

