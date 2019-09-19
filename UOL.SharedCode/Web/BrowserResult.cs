namespace UOL.SharedCode.Web
{
	public enum BrowserResultType
	{
		UnknownError,
		Success,
		Timeout
	}

	public class BrowserResult
	{
		public string Response { get; set; }

		public BrowserResultType ResultType { get; set; }

		public string Error { get; internal set; }
	}
}