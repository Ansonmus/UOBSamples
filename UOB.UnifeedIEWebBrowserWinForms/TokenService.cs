namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System.IO;
	using System.Net;
	using System.Text;

	using Newtonsoft.Json;

	public static class TokenService
	{
		internal static OAuthToken Get(string autorizeServer, string postMessage)
		{
			// Create Object's
			var request = WebRequest.CreateHttp(autorizeServer);

			// Set request data
			var data = Encoding.UTF8.GetBytes(postMessage);

			// Set request method, contentType and contentLength
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
			request.ContentLength = data.Length;

			using (var stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}

			// Get response
			var response = (HttpWebResponse)request.GetResponse();
			var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

			// Save response
			var token = JsonConvert.DeserializeObject<OAuthToken>(responseString);

			return token;
		}
	}
}
