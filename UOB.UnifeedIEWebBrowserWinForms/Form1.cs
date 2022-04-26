﻿//https://weblog.west-wind.com/posts/2021/Jan/14/Taking-the-new-Chromium-WebView2-Control-for-a-Spin-in-NET-Part-1

//#define BETA
namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Threading.Tasks;
	using System.Web;
	using System.Windows.Forms;
	using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
	using Microsoft.Web.WebView2.Core;
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
		public static bool EmbeddedAuth = true;
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

			Log($"Hello world!");


			authService = new Authentication(new AuthenticationConfig()
			{
				AuthorizeUrl = AuthorizeUrl,
				AuthorizeHookUrl = AuthorizeHookUrl,
				AuthorizeListenerAddress = AuthorizeListenerAddress,
				AuthorizeTokenUrl = AuthorizeTokenUrl,
				ClientId = ClientId,
				RequestedScope = "unifeed openid offline_access apix",
			});

			if (browser != null)
			{
				browser.NavigationStarting += Browser_NavigationStarting;

				//if (browser.Controls[0] is Microsoft.Toolkit.Forms.UI.Controls.WebView webView)
				//{
				//	webView.UnsupportedUriSchemeIdentified += WebView_UnsupportedUriSchemeIdentified;
				//	webView.NewWindowRequested += WebView_NewWindowRequested;
				//	webView.NavigationStarting += WebView_NavigationStarting;
				//}
				//else if(browser.Controls[0] is WebBrowser webBrowser)
				//{
				//	webBrowser.ScriptErrorsSuppressed = false;
				//	//browser.NavigationStarting += WebView_NavigationStarting;
				//}
				////webView.NavigationCompleted += WebView_NavigationCompleted;
			}
		}

		private async void Browser_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
		{
			// Log($"WebView_NavigationStarting: {e.Uri}");
			var uri = new Uri(e.Uri);
			if (uri.Scheme == UnifeedSchemeName)
			{
				e.Cancel = true;
				Log($"Interfaced (through WebViewCmopatible.NavigationStarting)! {e.Uri}");

				await UnifeedInterfaced(uri);
			}
			else if (uri.AbsolutePath.EndsWith("account/ForgotPasswordConfirmation", StringComparison.OrdinalIgnoreCase))
			{
				e.Cancel = true;
				await Authenticate();
				BeginInvoke(new Action(() =>
				{
					MessageBox.Show("An email is on it's way to your mailbox with instructions on how to reset your password.", "Password reset requested", MessageBoxButtons.OK);
				}));
			}
		}

		private async void Form1_Load(object sender, EventArgs e)
		{
			string webViewVersionAvail = string.Empty;
			Version asmversion;
			try
			{
				webViewVersionAvail = CoreWebView2Environment.GetAvailableBrowserVersionString();

				asmversion = typeof(CoreWebView2Environment).Assembly.GetName().Version;
			}
			catch { }


			await browser.EnsureCoreWebView2Async();
			browser.CoreWebView2.Settings.IsStatusBarEnabled = true;
			//browser.CoreWebView2.Settings.
			//browser.CoreWebView2.BrowserProcessId

			Log($"Form loaded. Webbrowser type: {this.browser}");
			Log($"Starting authentication");
			await Authenticate();
		}

		private async Task Authenticate()
		{
			_currentToken = TokenRepository.GetToken();

			if ( _currentToken != null && _currentToken.Environment == AuthorizeBaseUrl && _currentToken.IsExpired)
			{
				try
				{
					RefreshToken();
				}
				catch (Exception)
				{
					TokenRepository.StoreToken(null);
					await Authenticate();
				}
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
				{ "uobApplication", "DemoCad" },
				{ "uobApplicationVersion", "2022" },
				{ "hookUrl", UnifeedHookUrl },

			}).ToString();

			Log($"Starting Unifeed with url: {url}");

			Navigate(url);
			return;
		}


		private async void WebView_UnsupportedUriSchemeIdentified(object sender, WebViewControlUnsupportedUriSchemeIdentifiedEventArgs e)
		{
			// Log($"Browser_UnsupportedUriSchemeIdentified: {e.Uri}");
			if (e.Uri.Scheme == UnifeedSchemeName)
			{
				e.Handled = true;
				Log($"Interfaced (through WebView.UnsupportedUriSchemeIdentified)! {e.Uri}");

				await UnifeedInterfaced(e.Uri);
			}
		}

		private async void WebView_NavigationStarting(object sender, WebViewControlNavigationStartingEventArgs e)
		{
			// Log($"WebView_NavigationStarting: {e.Uri}");
			if (e.Uri.Scheme == UnifeedSchemeName)
			{
				e.Cancel = true;
				Log($"Interfaced (through WebViewCmopatible.NavigationStarting)! {e.Uri}");

				await UnifeedInterfaced(e.Uri);
			}
			else if (e.Uri.AbsolutePath.EndsWith("account/ForgotPasswordConfirmation", StringComparison.OrdinalIgnoreCase))
			{
				e.Cancel = true;
				await Authenticate();
				BeginInvoke(new Action(() =>
				{
					MessageBox.Show("An email is on it's way to your mailbox with instructions on how to reset your password.", "Password reset requested", MessageBoxButtons.OK);
				}));
			}
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
			var interfaceInfo = JsonConvert.DeserializeAnonymousType(data, new { Id = 0, Type = (string)null, DisableFields = (ICollection<BBA.UnifeedApi.ProductAttribute>)null });
			Log($"Retrieved id for interface object: {interfaceInfo.Id}. Type: {interfaceInfo.Type}");

			// Build URL to call ProductSelection Service
			var url = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrlNew}/api/v1/unifeed/UobProduct/FromSelectionList").ToString();

			var selectionListRequest = new BBA.UnifeedApi.SelectionListParms()
			{
				SelectionListId = interfaceInfo.Id,
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

		private async void Navigate(string url)
		{
			await browser.EnsureCoreWebView2Async();
			browser.CoreWebView2.Navigate(url);
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

//		private bool IsWebViewVersionInstalled(bool showDownloadUi = false)
//		{
//			string versionNo = null;
//			Version asmVersion = null;
//			Version ver = null;

//			try
//			{
//				versionNo = CoreWebView2Environment.GetAvailableBrowserVersionString();

//				// strip off 'canary' or 'stable' verison
//				versionNo = StringUtils.ExtractString(versionNo, "", " ", allowMissingEndDelimiter: true)?.Trim();
//				ver = new Version(versionNo);

//				asmVersion = typeof(CoreWebView2Environment).Assembly.GetName().Version;

//				if (ver.Build >= asmVersion.Build)
//					return true;
//			}
//			catch { }

//			IsActive = false;

//			if (!showDownloadUi)
//				return false;


//			var form = new BrowserMessageBox()
//			{
//				Owner = mmApp.Model.Window,
//				Width = 600,
//				Height = 440,
//				Title = "WebView Runtime Installation",
//			};

//			form.Dispatcher.Invoke(() => form.Icon = new ImageSourceConverter()
//				.ConvertFromString("pack://application:,,,/WebViewPreviewerAddin;component/icon_32.png") as ImageSource);

//			var markdown = $@"
//### WebView Runtime not installed or out of Date
//The Microsoft Edge WebView Runtime is
//{ (!string.IsNullOrEmpty(versionNo) ?
//				"out of date\n\nYour Build: " + ver.Build +
//				"   -   Required Build: " + asmVersion.Build :
//				"not installed")  }.

//In order to use the Chromium preview you need to install this runtime by downloading from the [Microsoft Download Site](https://developer.microsoft.com/en-us/microsoft-edge/webview2/).

//**Do you want to download and install the Edge WebView Runtime?**

//*<small>clicking **Yes** sends you to the Microsoft download site.  
//choose the **Evergreen Bootstrapper** download.</small>*";

//			form.ClearButtons();
//			var yesButton = form.AddButton("Yes", FontAwesomeIcon.CheckCircle, Brushes.Green);
//			yesButton.Width = 90;
//			var noButton = form.AddButton("No", FontAwesomeIcon.TimesCircle, Brushes.Firebrick);
//			noButton.Width = 90;
//			form.ShowMarkdown(markdown);


//			form.ShowDialog();
//			if (form.ButtonResult == yesButton)
//			{
//				mmFileUtils.OpenBrowser("https://developer.microsoft.com/en-us/microsoft-edge/webview2/");
//			}

//			return false;
//		}

	}
}
