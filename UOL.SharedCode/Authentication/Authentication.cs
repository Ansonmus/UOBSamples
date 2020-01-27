namespace UOL.SharedCode.Authentication
{
	using System;
	using System.Collections.Specialized;
	using System.Threading.Tasks;
	using System.Web;

	public class Authentication
	{
		private readonly AuthenticationConfig config;

		public Authentication(AuthenticationConfig config)
		{
			this.config = config;
		}

		public (string, PKCECode) BuildAuthorizationCodeUrl()
		{
			// First generate the PKCE verifier and challenge
			var pkceCodes = PKCECode.GeneratePKCECodes();

			var url = Web.HttpExtensions.Build(config.AuthorizeUrl, new NameValueCollection()
				{
					{ "response_type", "code" },
					{ "client_id", config.ClientId },
					{ "redirect_uri", config.AuthorizeHookUrl },
					{ "scope", config.RequestedScope },
					{ "code_challenge", pkceCodes.CodeChallenge }, // PKCE addition
					{ "code_challenge_method", "S256" }, // PKCE addition
				}).ToString();

			return (url, pkceCodes);
		}

		public async Task<OAuthToken> Authenticate()
		{
			(var url, var pkceCodes) = BuildAuthorizationCodeUrl();

			// This best current practice requires that only external user-agents
			// like the browser are used for OAuth by native apps
			// rfc8252 - https://tools.ietf.org/html/rfc8252#section-1
			var x = new Web.SystemBrowser(config.AuthorizeListenerAddress);
			var response = await x.InvokeAsync(url);

			var uri = new Uri(response.Response);
			if (response.ResultType == Web.BrowserResultType.Success)
			{
				// Call the token service with the authorization code (and PKCE verifier)
				var queryString = HttpUtility.ParseQueryString(uri.Query);
				var authorizationcode = queryString.Get("code");

				var query = Web.HttpExtensions.BuildQuerystring(new NameValueCollection()
				{
					{ "grant_type", "authorization_code" },
					{ "code", authorizationcode },
					{ "client_id", config.ClientId },
					{ "redirect_uri", config.AuthorizeHookUrl },
					{ "code_verifier", pkceCodes.CodeVerifier }, // PKCE addition
				}).ToString();

				return TokenService.RetrieveToken(config.AuthorizeTokenUrl, query);
			}

			return null;
		}
	}
}
