namespace TntMPDConverter.Properties
{
	using System;
	using System.CodeDom.Compiler;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Globalization;
	using System.Resources;
	using System.Runtime.CompilerServices;

	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"), CompilerGenerated, DebuggerNonUserCode]
	internal class Resources
	{
		internal Resources()
		{
		}


		[EditorBrowsable(EditorBrowsableState.Advanced)]
		static internal CultureInfo Culture
		{
			get { return Resources.resourceCulture; }
			set { Resources.resourceCulture = value; }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		static internal System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					System.Resources.ResourceManager manager1 = new System.Resources.ResourceManager("TntMPDConverter.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = manager1;
				}
				return Resources.resourceMan;
			}
		}


		private static CultureInfo resourceCulture;
		private static System.Resources.ResourceManager resourceMan;
	}
}

