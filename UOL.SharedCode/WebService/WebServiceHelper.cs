namespace UOL.SharedCode.WebService
{
	using System.Collections.Specialized;
	using System.IO;
	using System.Net;
	using Newtonsoft.Json;

	public class WebServiceHelper
	{
		public static Models.Interface GetInterfaceObject(string apiUrl, string accessToken, int id)
		{
			var url = Web.HttpExtensions.Build($"{apiUrl}/json/UOB/Interface", new NameValueCollection()
			{
				{ "id", id.ToString() },
			}).ToString();

			var data = GetJson(url, accessToken);
			return JsonConvert.DeserializeObject<SharedCode.Models.Interface>(data);
		}

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
}
