namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Newtonsoft.Json;

	internal class UnifeedHelper
	{
		public static string InitializeWorkingFile(string url, object postData)
		{
			var path = GetWorkingFile();
			using (var file = File.CreateText(path))
			{
				foreach (var line in GetHtml(url, postData).Split('\n'))
				{
					file.WriteLine(line);
				}
			}

			return path;
		}

		private static string GetWorkingFile()
		{
			return Path.Combine(Path.GetTempPath(), "bbapostform.html");
		}

		private static string GetHtml(string url, object postData)
		{
			var retval = $@"
			<!DOCTYPE html>
			<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>
			<head>
				<meta charset='utf-8' />
				<title>Please wait...</title>
			</head>
			<body>
				<form method='post' action='{url}' id='postform'>
";
			foreach (var propinfo in postData.GetType().GetProperties())
			{
				var rawValue = propinfo.GetValue(postData);

				string value;
				if (rawValue.GetType() == typeof(string))
				{
					value = (string)rawValue;
				}
				else
				{
					value = JsonConvert.SerializeObject(rawValue);
				}

				retval += $"<input type='hidden' name='{propinfo.Name}' value='{value}' />\n";
			}

			retval += @"</form>
				<script>
					document.getElementById('postform').submit();
				</script>
			</body>
			</html>";

			return retval;
		}
	}
}
