namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.Net;
	using System.Net.Sockets;
	using System.Runtime.InteropServices;
	using System.Web;

	public static class HttpExtensions
	{
		public static Uri AddQuery(this Uri uri, string name, string value)
		{
			var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

			httpValueCollection.Remove(name);
			httpValueCollection.Add(name, value);

			var ub = new UriBuilder(uri)
			{
				Query = httpValueCollection.ToString()
			};

			return ub.Uri;
		}

		public static Uri Build(string baseUrl, NameValueCollection parameters)
		{
			var uri = new Uri(baseUrl);
			var queryString = HttpUtility.ParseQueryString(uri.Query);
			if (parameters != null)
			{
				queryString.Add(parameters);
			}

			var ub = new UriBuilder($"{uri.Scheme}{Uri.SchemeDelimiter}{uri.Authority}{uri.AbsolutePath}")
			{
				Query = queryString.ToString()
			};
			return ub.Uri;
		}

		public static string BuildQuerystring(NameValueCollection parameters)
		{
			var queryString = HttpUtility.ParseQueryString(string.Empty);
			queryString.Add(parameters);
			return queryString.ToString();
		}
	}
}
