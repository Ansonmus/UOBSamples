namespace UOL.SharedCode.IO
{
	using System;
	using System.IO;
	using System.Reflection;

	public class ApplicationHelper
	{
		public static string AssemblyDirectory
		{
			get
			{
				var codeBase = Assembly.GetExecutingAssembly().CodeBase;
				var uri = new UriBuilder(codeBase);
				var path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}
	}
}
