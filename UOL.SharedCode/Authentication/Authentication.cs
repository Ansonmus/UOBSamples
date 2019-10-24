namespace UOL.SharedCode.Authentication
{
	using System;
	using System.Collections.Specialized;
	using System.Threading.Tasks;
	using System.Web;

	public class Authentication
	{
		private readonly AuthenticationConfig _config;

		public Authentication(AuthenticationConfig config)
		{
			_config = config;
		}

		public async Task<OAuthToken> Authenticate()
		{
			// First generate the PKCE verifier and challenge
			var pkceCodes = PKCECode.GeneratePKCECodes();

			var url = Web.HttpExtensions.Build(_config.AuthorizeUrl, new NameValueCollection()
				{
					{ "response_type", "code" },
					{ "client_id", _config.ClientId },
					{ "redirect_uri", _config.AuthorizeHookUrl },
					{ "scope", _config.RequestedScope },
					{ "code_challenge", pkceCodes.CodeChallenge }, // PKCE addition
					{ "code_challenge_method", "S256" }, // PKCE addition
				}).ToString();

			// This best current practice requires that only external user-agents
			// like the browser are used for OAuth by native apps
			// rfc8252 - https://tools.ietf.org/html/rfc8252#section-1
			var x = new Web.SystemBrowser(_config.AuthorizeListenerAddress);
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
					{ "client_id", _config.ClientId },
					{ "redirect_uri", _config.AuthorizeHookUrl },
					{ "code_verifier", pkceCodes.CodeVerifier }, // PKCE addition
				}).ToString();

				return TokenService.RetrieveToken(_config.AuthorizeTokenUrl, query);
			}

			return null;
		}
	}
}
