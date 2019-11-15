
namespace UOL.SharedCode.Authentication
{
	using Newtonsoft.Json;

	public class OAuthToken
	{
		/// <summary>
		/// Gets or sets the access token issued by the authorization server.
		/// </summary>
		[JsonProperty(PropertyName = "access_token")]
		public string AccessToken { get; set; }

		/// <summary>
		/// Gets or sets the lifetime in seconds of the access token.  For example, the value "3600" denotes that the access token will expire in one hour from the time the response was generated.
		/// </summary>
		[JsonProperty(PropertyName = "expires_in")]
		public int ExpiresIn { get; set; }

		public System.DateTime TokenIssued { get; } = System.DateTime.Now;

		/// <summary>
		/// Gets or sets the type of the token issued. Value is case insensitive (see https://tools.ietf.org/html/draft-ietf-oauth-v2-22#section-7.1)
		/// </summary>
		[JsonProperty(PropertyName = "token_type")]
		public string TokenType { get; set; }

		/// <summary>
		/// Gets or sets the refresh token which can be used to obtain new access tokens using the same authorization grant as described
		/// </summary>
		[JsonProperty(PropertyName = "refresh_token")]
		public string RefreshToken { get; set; }

		public bool IsExpired()
		{
			return TokenIssued.AddSeconds(ExpiresIn) > System.DateTime.Now;
		}
	}
}