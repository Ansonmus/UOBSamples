namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;
	using System.Web;
	using System.Windows.Forms;
	using Newtonsoft.Json;

	public partial class Form1 : Form
	{
		public const string ClientId = "2BA_DEMOAPPS_PKCE";
		public const string AuthorizeBaseUrl = "https://authorize-uol.2ba.nl";
		public const string UnifeedBaseUrl = "https://unifeed-uol.alpha.2ba.nl";
		public const string UnifeedSchemeName = "nl.2ba.uol";
		public static readonly string AuthorizeUrl = $"{AuthorizeBaseUrl}/OAuth/Authorize";
		public static readonly string AuthorizeTokenUrl = $"{AuthorizeBaseUrl}/OAuth/Token";
		public static readonly string AuthorizeListenerAddress = $"http://localhost:43215/"; // Must end with slash
		public static readonly string AuthorizeHookUrl = $"{AuthorizeListenerAddress}tokenreceiver"; // = Redirect_uri as configured for client
		public static readonly string UnifeedHookUrl = $"{UnifeedSchemeName}://request";
		public static readonly string UnifeedStartUrl = $"{UnifeedBaseUrl}/start";
		public static readonly string UnifeedApiUrl = $"{UnifeedBaseUrl}/api";

		private OAuthToken _currentToken = null;

		public Form1()
		{
			InitializeComponent();
		}

		private async void Form1_Load(object sender, EventArgs e)
		{
			Log($"Form loaded. sttarting authenticate");
			_currentToken = await Authenticate();

			Log($"Authentication complete, starting Unifeed");
			var searchParams = new UnifeedObjects.SearchParms()
			{
				From = 0,
				Size = 20,
				Languagecode = "NL",
				SearchString = "pomp"
			};

			StartUnifeed(searchParams);
		}

		private async Task<OAuthToken> Authenticate()
		{
			// First generate the PKCE verifier and challenge
			PKCECode pkceCodes = PKCECode.GeneratePKCECodes();

			var url = HttpExtensions.Build(AuthorizeUrl, new NameValueCollection()
				{
					{ "response_type", "code" },
					{ "client_id", ClientId },
					{ "redirect_uri", AuthorizeHookUrl },
					{ "code_challenge", pkceCodes.CodeChallenge }, // PKCE addition
					{ "code_challenge_method", "S256" }, // PKCE addition
				}).ToString();

			// This best current practice requires that only external user-agents
			// like the browser are used for OAuth by native apps
			// rfc8252 - https://tools.ietf.org/html/rfc8252#section-1
			var x = new SystemBrowser(AuthorizeListenerAddress);
			var response = await x.InvokeAsync(url);

			var uri = new Uri(response.Response);
			if (response.ResultType == BrowserResultType.Success)
			{
				// Call the token service with the authorization code (and PKCE verifier)
				var queryString = HttpUtility.ParseQueryString(uri.Query);
				var authorizationcode = queryString.Get("code");

				var query = HttpExtensions.BuildQuerystring(new NameValueCollection()
				{
					{ "grant_type", "authorization_code" },
					{ "code", authorizationcode },
					{ "client_id", ClientId },
					{ "redirect_uri", AuthorizeHookUrl },
					{ "code_verifier", pkceCodes.CodeVerifier }, // PKCE addition
				}).ToString();

				return TokenService.Get(AuthorizeTokenUrl, query);
			}

			return null;
		}

		private void StartUnifeed(UnifeedObjects.SearchParms searchParms = null)
		{
			// Since there is a problem with /start we temporary call the base page
			//browser.Navigate(UnifeedBaseUrl);
			//return;

			var accessToken = _currentToken.AccessToken;
			var url = HttpExtensions.Build(UnifeedStartUrl, new NameValueCollection()
			{
				//{ "accessToken", accessToken },
				{ "hookUrl", UnifeedHookUrl },
			}).ToString();

			//var postData = JsonConvert.SerializeObject(searchParms);
			//var path = UnifeedHelper.InitializeWorkingFile(url, new { searchParms = postData });
			//browser.Navigate(path);

			if (searchParms != null)
			{

				var data = @"{ 
   ""from"":20,
   ""size"":20,
   ""languagecode"":""NL"",
   ""searchString"":""uob"",
   ""filters"":[ 
      { 
         ""code"":""ModellingClass"",
         ""values"":[ 
            { 
               ""code"":""MC000178""
            }
         ]
      }
   ]
}";

				data = @"{ 
   ""from"":20,
   ""size"":20,
   ""languagecode"":""NL"",
   ""searchString"":""lamp""
}";
				//data = JsonConvert.SerializeObject(searchParms);
				byte[] postdata = Encoding.UTF8.GetBytes(data);
				string headers = $"Content-Type: application/json";
				browser.Navigate(url, string.Empty, postdata, headers);
			}
			else
			{
				browser.Navigate(url);
			}
		}

		private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			Log($"Document completed: {browser.DocumentType}");
		}

		private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			Log($"Navigating to: {e.Url}");
			if (e.Url.Scheme == UnifeedSchemeName)
			{
				Log($"Interfaced! {e.Url}");

				// Retrieve object at the Unifeed API
				string json;
				using (var wc = new WebClient())
				{
					json = wc.DownloadString($"{UnifeedApiUrl}{e.Url.AbsolutePath}");
				}

				Log($"Retrieved object: {json}");

				// Refresh token. Normally this is not needed for every call, only when the token is expired.
				RefreshToken();

				// Restart Unifeed
				StartUnifeed(); // browser.Refresh();
			}
		}

		private void Log(string tekst)
		{
			logBox.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {Environment.NewLine}";
			System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {Environment.NewLine}");
		}

		private void RefreshToken()
		{
			Log($"Refreshing tokens. Old token retrieved: {_currentToken.TokenIssued.ToString("yyyyMMdd_HHmmss")}");
			var query = HttpExtensions.BuildQuerystring(new NameValueCollection()
				{
					{ "client_id", ClientId },
					{ "grant_type", "refresh_token" },
					{ "refresh_token", _currentToken.RefreshToken },
				}).ToString();

			_currentToken = TokenService.Get(AuthorizeTokenUrl, query);

			Log($"Refreshing tokens. New token retrieved: {_currentToken.TokenIssued.ToString("yyyyMMdd_HHmmss")}");
		}
	}
}
