namespace UOL.SharedCode.Authentication
{
	public class AuthenticationConfig
	{
		public string AuthorizeUrl { get; set; }

		public string AuthorizeHookUrl { get; set; }

		public string AuthorizeListenerAddress { get; set; }

		public string AuthorizeTokenUrl { get; set; }

		public string ClientId { get; set; }

		public string RequestedScope { get; set; }
	}
}
