#define BETA
namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using System.Collections.Specialized;
	using System.Threading;
	using System.Web;
	using System.Windows.Forms;
	using CefSharp;
	using CefSharp.WinForms.Internals;
	using Newtonsoft.Json;

	public partial class Form1 : Form
	{
		public const string ClientId = "2BA_DEMOAPPS_PKCE";

#if BETA
		public const string AuthorizeBaseUrl = "https://uol-auth.beta.2ba.nl";
		public const string UnifeedBaseUrl = "https://uol-unifeed.beta.2ba.nl";
		public const string ApiBaseUrl = "https://uol-api.beta.2ba.nl/1";
#else
		public const string AuthorizeBaseUrl = "https://uol-auth.2ba.nl";
		public const string UnifeedBaseUrl = "https://uol-unifeed.2ba.nl";
		public const string ApiBaseUrl = "https://uol-api.2ba.nl/1";
#endif

		public const string UnifeedSchemeName = "nl.2ba.uol";
		public static readonly string AuthorizeUrl = $"{AuthorizeBaseUrl}/OAuth/Authorize";
		public static readonly string AuthorizeTokenUrl = $"{AuthorizeBaseUrl}/OAuth/Token";
		public static readonly string AuthorizeListenerAddress = $"http://localhost:43215/"; // Must end with slash
		public static readonly string AuthorizeHookUrl = $"{AuthorizeListenerAddress}tokenreceiver"; // = Redirect_uri as configured for client
		public static readonly string UnifeedHookUrl = $"{UnifeedSchemeName}://request";

		private SharedCode.Authentication.OAuthToken _currentToken = null;

		public Form1()
		{
			InitializeComponent();
			browser.ScriptErrorsSuppressed = false;
		}

		private async void Form1_Load(object sender, EventArgs e)
		{
			Log($"Form loaded. starting authenticate");
			var authService = new SharedCode.Authentication.Authentication(new SharedCode.Authentication.AuthenticationConfig()
			{
				AuthorizeUrl = AuthorizeUrl,
				AuthorizeHookUrl = AuthorizeHookUrl,
				AuthorizeListenerAddress = AuthorizeListenerAddress,
				AuthorizeTokenUrl = AuthorizeTokenUrl,
				ClientId = ClientId,
				RequestedScope = "unifeed openid offline_access",
			});

			_currentToken = await authService.Authenticate();

			RegisterUnifeedSchemaHandler();

			Log($"Authentication complete, starting Unifeed");
			StartUnifeed(BrowserType.InternetExplorer | BrowserType.Chromium);
		}

		private void RegisterUnifeedSchemaHandler()
		{
			var factory = new CustomSchemeHandlerFactory(UnifeedSchemeName);
			factory.CustomSchemeTriggered += Factory_UnifeedInterfaceTriggered;
			var settings = new CefSharp.WinForms.CefSettings();
			settings.RegisterScheme(new CefCustomScheme()
			{
				SchemeName = factory.SchemeName,
				SchemeHandlerFactory = factory
			});

			Cef.Initialize(settings);
		}

		private void Factory_UnifeedInterfaceTriggered(object sender, UnifeedSchemaHandlerEventArgs e)
		{
			HandleInterfaceReceived(new Uri(e.Request.Url), BrowserType.Chromium);
		}

		private void StartUnifeed(BrowserType browserType, int? interfaceObjectId = null)
		{
			var accessToken = _currentToken.AccessToken;

			var downloadUrl = SharedCode.Web.HttpExtensions.Build(UnifeedBaseUrl, new NameValueCollection()
			{
				{ "accessToken", accessToken },
				{ "interface", 32.ToString() },
				{ "interfaceType", "DOWNLOAD" },
				{ "interfaceName", "lokaal apparaat (download)" },
				{ "uobApplication", "Revit" },
				{ "uobApplicationVersion", "2018" },
				{ "interfaceObjectId", interfaceObjectId?.ToString() },
			}).ToString();

			Log($"Link to initiate download: {downloadUrl}");

			var url = SharedCode.Web.HttpExtensions.Build(UnifeedBaseUrl, new NameValueCollection()
			{
				{ "accessToken", accessToken },
				{ "interface", 32.ToString() },
				{ "interfaceType", "JSONGET" },
				{ "interfaceName", "UOB DemoApplication" },
				{ "hookUrl", UnifeedHookUrl },
			}).ToString();

			if (browserType.HasFlag(BrowserType.InternetExplorer))
			{
				browser.Navigate(url);
			}

			if (browserType.HasFlag(BrowserType.Chromium))
			{
				this.chromBrowser.Load(url);
			}

			return;
		}

		private async void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			// Restart situation
			if (!string.IsNullOrEmpty(_currentToken.RefreshToken))
			{
				// Refresh token. Normally this is not needed for every call, only when the token is expired.
				// Only possible when offline_access scope is honored
				RefreshToken();
			}

			Log($"Navigating to: {e.Url}");
			if (e.Url.Scheme == UnifeedSchemeName)
			{
				e.Cancel = true;
				HandleInterfaceReceived(e.Url, BrowserType.InternetExplorer);
			}
		}

		private async void HandleInterfaceReceived(Uri uri, BrowserType browserType)
		{
			Log($"Interfaced! {uri}");

			// Get id from data
			var queryString = HttpUtility.ParseQueryString(uri.Query);
			string data = queryString["json"];
			var interfaceId = JsonConvert.DeserializeAnonymousType(data, new { Id = 0L }).Id;
			Log($"Retrieved id for interface object: {interfaceId}");

			// Call interface webservice
			var url = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrl}/json/UOB/Interface", new NameValueCollection()
				{
					{ "id", interfaceId.ToString() },
				}).ToString();

			Log($"Calling service url: {url}");

			var interfaceObjectJson = SharedCode.WebService.WebServiceHelper.GetJson(url, _currentToken.AccessToken);
			Log($"Retrieved interface object: {Newtonsoft.Json.Linq.JToken.Parse(interfaceObjectJson).ToString(Formatting.Indented)}");

			var interfaceObject = JsonConvert.DeserializeObject<SharedCode.Models.Interface>(interfaceObjectJson);

			// Post the interfaceObject back
			var postUrl = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrl}/json/UOB/Interface").ToString();
			var output = await SharedCode.WebService.WebServiceHelper.PostJson(postUrl, _currentToken.AccessToken, interfaceObjectJson);

			if (int.TryParse(output, out var newInterfaceId))
			{
				// Restart Unifeed
				StartUnifeed(browserType, newInterfaceId); // browser.Refresh();
			}
			else
			{
				// Restart Unifeed
				StartUnifeed(browserType); // browser.Refresh();
			}
		}

		private void Browser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
		{
			// Log($"Navigated to: {e.Url}");
		}

		private void Log(string tekst)
		{
			this.InvokeOnUiThreadIfRequired(() =>
			{
				logBox.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {Environment.NewLine}";
				System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {Environment.NewLine}");
			});

		}

		private void RefreshToken()
		{
			Log($"Refreshing tokens. Old token retrieved: {_currentToken.TokenIssued.ToString("yyyyMMdd_HHmmss")}");
			var query = SharedCode.Web.HttpExtensions.BuildQuerystring(new NameValueCollection()
				{
					{ "client_id", ClientId },
					{ "grant_type", "refresh_token" },
					{ "refresh_token", _currentToken.RefreshToken },
				}).ToString();

			_currentToken = SharedCode.Authentication.TokenService.RetrieveToken(AuthorizeTokenUrl, query);

			Log($"Refreshing tokens. New token retrieved: {_currentToken.TokenIssued.ToString("yyyyMMdd_HHmmss")}");
		}
	}
}
