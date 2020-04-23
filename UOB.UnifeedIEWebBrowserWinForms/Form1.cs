#define BETA
namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Threading.Tasks;
	using System.Web;
	using System.Windows.Forms;
	using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
	using Newtonsoft.Json;
	using UOL.SharedCode.Authentication;

	public partial class Form1 : Form
	{
		public const string ClientId = "2BA_DEMOAPPS_PKCE";

#if ALPHA
		public const string AuthorizeBaseUrl = "https://authorize.alpha.2ba.nl";
		public const string UnifeedBaseUrl = "https://uol-unifeed.alpha.2ba.nl";
		public const string ApiBaseUrlNew = "https://apix.alpha.2ba.nl";
#elif BETA
		public const string AuthorizeBaseUrl = "https://uol-auth.beta.2ba.nl";
		public const string UnifeedBaseUrl = "https://uol-unifeed.beta.2ba.nl";
		public const string ApiBaseUrlNew = "https://apix.beta.2ba.nl";
#else
		public const string AuthorizeBaseUrl = "https://uol-auth.2ba.nl";
		public const string UnifeedBaseUrl = "https://uol-unifeed.2ba.nl";
		public const string ApiBaseUrlNew = "https://apix.2ba.nl";
#endif
		public static bool UseWebView = true;
		public static bool EmbeddedAuth = UseWebView;
		public const string UnifeedSchemeName = "nl.2ba.uol";
		public static readonly string AuthorizeUrl = $"{AuthorizeBaseUrl}/OAuth/Authorize";
		public static readonly string AuthorizeTokenUrl = $"{AuthorizeBaseUrl}/OAuth/Token";
		public static readonly string AuthorizeListenerAddress = EmbeddedAuth ? $"{UnifeedSchemeName}://" : $"http://localhost:43215/"; // Must end with slash
		public static readonly string AuthorizeHookAuthority = "tokenreceiver";
		public static readonly string AuthorizeHookUrl = $"{AuthorizeListenerAddress}{AuthorizeHookAuthority}"; // = Redirect_uri as configured for client
		public static readonly string UnifeedHookUrl = $"{UnifeedSchemeName}://request";

		private OAuthToken _currentToken = null;
		private PKCECode _pkcetemp = null;

		private BBA.UnifeedApi.InterfaceModel _lastRetrievedObject = null;

		private readonly Authentication authService = null;

		public Form1()
		{
			InitializeComponent();

			authService = new Authentication(new AuthenticationConfig()
			{
				AuthorizeUrl = AuthorizeUrl,
				AuthorizeHookUrl = AuthorizeHookUrl,
				AuthorizeListenerAddress = AuthorizeListenerAddress,
				AuthorizeTokenUrl = AuthorizeTokenUrl,
				ClientId = ClientId,
				RequestedScope = "unifeed openid offline_access apix",
			});

			webView.NavigationStarting += WebView_NavigationStarting;
			webView.NavigationCompleted += WebView_NavigationCompleted;
			webView.UnsupportedUriSchemeIdentified += WebView_UnsupportedUriSchemeIdentified;
			webView.NewWindowRequested += WebView_NewWindowRequested;

			browser.Navigating += Browser_Navigating;
			browser.Navigated += Browser_Navigated;
			browser.ScriptErrorsSuppressed = false;

			if (UseWebView)
			{
				webView.Visible = true;
				browser.Visible = false;
			}
			else
			{
				webView.Visible = false;
				browser.Visible = true;
			}
		}

		private async void Form1_Load(object sender, EventArgs e)
		{
			Log($"Form loaded. starting authenticate");
			await Authenticate();
		}

		private async Task Authenticate()
		{
			_currentToken = TokenRepository.GetToken();

			if ( _currentToken != null && _currentToken.Environment == AuthorizeBaseUrl && _currentToken.IsExpired)
			{
				RefreshToken();
			}
			else if (_currentToken == null || _currentToken.Environment != AuthorizeBaseUrl)
			{
				if (EmbeddedAuth)
				{
					(string url, PKCECode pkcecodes) x = authService.BuildAuthorizationCodeUrl();
					_pkcetemp = x.pkcecodes;
					Navigate(x.url);
				}
				else
				{
					_currentToken = await authService.Authenticate();
					_currentToken.Environment = AuthorizeBaseUrl;
					TokenRepository.StoreToken(_currentToken);
					AuthenticationComplete();
				}
			}
			else // Token set and not expired
			{
				AuthenticationComplete();
			}
		}

		private void AuthenticationComplete()
		{
			Log($"Authentication complete, starting Unifeed");
			StartUnifeed();
		}

		private void StartUnifeed(long? interfaceObjectId = null)
		{
			var accessToken = _currentToken.AccessToken;

			var url = SharedCode.Web.HttpExtensions.Build(UnifeedBaseUrl, new NameValueCollection()
			{
				{ "accessToken", accessToken },
				{ "interface", (32 | 64).ToString() },
				{ "interfaceObjectId", interfaceObjectId?.ToString() },
				{ "interfaceType", "JSONGET" },
				{ "interfaceName", "DemoApp" },
				{ "hookUrl", UnifeedHookUrl },

			}).ToString();

			Log($"Starting Unifeed with url: {url}");

			Navigate(url);
			return;
		}

		private async void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			// Log($"Navigating to: {e.Url}");
			if (e.Url.Scheme == UnifeedSchemeName)
			{
				e.Cancel = true;
				Log($"Interfaced! {e.Url}");

				await UnifeedInterfaced(e.Url);
			}
		}

		private async void WebView_UnsupportedUriSchemeIdentified(object sender, WebViewControlUnsupportedUriSchemeIdentifiedEventArgs e)
		{
			// Log($"Browser_UnsupportedUriSchemeIdentified: {e.Uri}");
			if (e.Uri.Scheme == UnifeedSchemeName)
			{
				e.Handled = true;
				Log($"Interfaced! {e.Uri}");

				await UnifeedInterfaced(e.Uri);
			}
		}

		private async void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			// Log($"Navigated to: {e.Url}");
		}

		private async void WebView_NavigationStarting(object sender, WebViewControlNavigationStartingEventArgs e)
		{
			// Log($"Navigating to: {e.Uri}");
		}

		private void WebView_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
		{
			// Log($"Navigated to: {e.Uri}");
		}

		private void WebView_NewWindowRequested(object sender, WebViewControlNewWindowRequestedEventArgs e)
		{
			Log($"Browser_NewWindowRequested: {e.Uri}");
			SharedCode.Web.SystemBrowser.OpenBrowser(e.Uri.ToString());
		}


		private async Task UnifeedInterfaced(Uri interfaceUrl)
		{
			var queryString = HttpUtility.ParseQueryString(interfaceUrl.Query);

			if (interfaceUrl.Authority == AuthorizeHookAuthority)
			{
				var code = queryString["code"];
				_currentToken = authService.RetrieveToken(code, _pkcetemp.CodeVerifier);
				_currentToken.Environment = AuthorizeBaseUrl;
				TokenRepository.StoreToken(_currentToken);
				AuthenticationComplete();
				return;
			}

			// Get id from data
			var data = queryString["json"];

			var interfaceBaseInfo = JsonConvert.DeserializeAnonymousType(data, new { Type = (string)null });
			Log($"Retrieved interfacetype: {interfaceBaseInfo.Type}");

			if ("interface".Equals(interfaceBaseInfo.Type, StringComparison.OrdinalIgnoreCase))
			{
				UnifeedInterfaceInterface(data);
			}
			else if ("productselection".Equals(interfaceBaseInfo.Type, StringComparison.OrdinalIgnoreCase))
			{
				await UnifeedInterfaceProductSelection(data);
			}

			// Restart situation
			if (_currentToken.IsExpired&& !string.IsNullOrEmpty(_currentToken.RefreshToken))
			{
				// Refresh token. Normally this is not needed for every call, only when the token is expired.
				// Only possible when offline_access scope is honored
				RefreshToken();
			}

			StartUnifeed(); // browser.Refresh();
		}

		private void UnifeedInterfaceInterface(string data)
		{
			var interfaceInfo = JsonConvert.DeserializeAnonymousType(data, new { Id = 0L, Type = (string)null });
			Log($"Retrieved id for interface object: {interfaceInfo.Id}. Type: {interfaceInfo.Type}");

			var url = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrlNew}/api/v1/unifeed/UobInterface/{interfaceInfo.Id}").ToString();

			Log($"Calling service url: {url}");

			var interfaceObjectJson = SharedCode.WebService.WebServiceHelper.GetJson(url, _currentToken.AccessToken);
			Log($"Retrieved interface object: {Newtonsoft.Json.Linq.JToken.Parse(interfaceObjectJson).ToString(Formatting.Indented)}");

			_lastRetrievedObject = JsonConvert.DeserializeObject<BBA.UnifeedApi.InterfaceModel>(interfaceObjectJson);
			btnStartWithLastObject.Enabled = true;
		}

		private async Task UnifeedInterfaceProductSelection(string data)
		{
			var interfaceInfo = JsonConvert.DeserializeAnonymousType(data, new { Id = 0, ObjectType = (string)null, Type = (string)null, DisableFields = (ICollection<BBA.UnifeedApi.ProductAttribute>)null });
			Log($"Retrieved id for interface object: {interfaceInfo.Id}. Type: {interfaceInfo.Type}");

			// Build URL to call ProductSelection Service
			var url = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrlNew}/api/v1/unifeed/UobProduct/FromSelectionList").ToString();

			var selectionListRequest = new BBA.UnifeedApi.SelectionListParms()
			{
				SelectionListId = interfaceInfo.Id,
				ObjectType = interfaceInfo.ObjectType,
				DisableFields = interfaceInfo.DisableFields,
				Languagecode = BBA.UnifeedApi.Languagecode.NL
			};
			var selectionListRequestString = JsonConvert.SerializeObject(selectionListRequest);

			// Call the service
			Log($"Calling service url: {url}");
			var interfaceObjectJson = await SharedCode.WebService.WebServiceHelper.PostJson(url, _currentToken.AccessToken, selectionListRequestString);

			// Deserialize
			var productlist = JsonConvert.DeserializeObject<List<BBA.UnifeedApi.ProductModel>>(interfaceObjectJson);
			Log($"Retrieved productselection object. Total products in list: {productlist.Count}");

			// Interface handling
			btnStartWithLastObject.Enabled = false;
		}

		private void Log(string tekst)
		{
			logBox.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {System.Environment.NewLine}";
			// System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {Environment.NewLine}");

			logBox.SelectionStart = logBox.Text.Length;
			logBox.ScrollToCaret();
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

			_currentToken = TokenService.RetrieveToken(AuthorizeTokenUrl, query);
			_currentToken.Environment = AuthorizeBaseUrl;
			TokenRepository.StoreToken(_currentToken);

			Log($"Refreshing tokens. New token retrieved: {_currentToken.TokenIssued.ToString("yyyyMMdd_HHmmss")}");
		}

		private void btnClearLog_Click(object sender, EventArgs e)
		{
			logBox.Text = string.Empty;
		}

		private async void btnStartWithLastObject_Click(object sender, EventArgs e)
		{
			if (_lastRetrievedObject == null)
			{
				btnStartWithLastObject.Enabled = false;
				return;
			}

			var interfaceObjectJson = JsonConvert.SerializeObject(_lastRetrievedObject);

			// Post the interfaceObject back
			//var postUrl = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrlOld}/json/UOB/Interface").ToString();
			var postUrl = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrlNew}/api/v1/unifeed/UobInterface").ToString();
			Log($"Posting object back to: {postUrl}");
			var output = await SharedCode.WebService.WebServiceHelper.PostJson(postUrl, _currentToken.AccessToken, interfaceObjectJson);

			Log($"Retrieved interface id: {output}");

			var lastInterfaceId = long.Parse(output);
			StartUnifeed(lastInterfaceId);
		}

		private void tbnStartClear_Click(object sender, EventArgs e)
		{
			StartUnifeed();
		}

		private async void btnReset_Click(object sender, EventArgs e)
		{
			Navigate("");
			TokenRepository.StoreToken(null);
			await Authenticate();
		}

		private void Navigate(string url)
		{
			if (UseWebView)
			{
				webView.Navigate(url);
			}
			else
			{
				browser.Navigate(url);
			}
		}

		private void btnDownload_Click(object sender, EventArgs e)
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
			}).ToString();

			Log($"Link to initiate download: {downloadUrl}");

			SharedCode.Web.SystemBrowser.OpenBrowser(downloadUrl);
		}
	}
}
