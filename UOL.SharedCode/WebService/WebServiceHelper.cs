namespace UOL.SharedCode.WebService
{
	using System;
	using System.Collections.Specialized;
	using System.IO;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading.Tasks;
	using Newtonsoft.Json;

	public class WebServiceHelper
	{
		public static string GetJson(string url, string accessToken)
		{
			var request = WebRequest.CreateHttp(url);

			// Set request settings
			request.Method = "GET";
			request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {accessToken}");

			// Get response
			var response = (HttpWebResponse)request.GetResponse();
			var dataStream = response.GetResponseStream();

			using (var reader = new StreamReader(dataStream))
			{
				var data = reader.ReadToEnd();

				ThrowOnError(response.StatusCode, data);

				return data;
			}
		}

		public static async Task<string> PostJson(string url, string accessToken, string jsonData)
		{
			var client = HttpClientHelpers.GetHttpClient();
			HttpRequestMessage request;

			request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

			if (!string.IsNullOrEmpty(accessToken))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			}

			var response = await client.SendAsync(request);
			return await response.Content.ReadAsStringAsync();
		}

		private static void ThrowOnError(HttpStatusCode statusCode, string responseString)
		{
			if (statusCode == HttpStatusCode.OK)
			{
				return;
			}

			var responseObject = JsonConvert.DeserializeObject<ErrorObject>(responseString);

			switch (statusCode)
			{
				case HttpStatusCode.BadRequest:
				case HttpStatusCode.Unauthorized:
					throw new WebException(responseObject.ErrorMessage, WebExceptionStatus.ProtocolError);
				case HttpStatusCode.InternalServerError:
					throw new WebException(responseObject.ErrorMessage, WebExceptionStatus.UnknownError);
			}
		}
	}

	internal static class HttpClientHelpers
	{
		private static HttpClient _client;

		static HttpClientHelpers()
		{
			_client = new HttpClient();
		}

		internal static HttpClient GetHttpClient() => _client;

		internal static TimeSpan DefaultTimeOut => TimeSpan.FromSeconds(100);

		internal static void SetTimeout(TimeSpan timeSpan)
		{
			_client.Timeout = timeSpan;
		}

		internal static void ResetTimeout()
		{
			SetTimeout(DefaultTimeOut);
		}
	}
}
