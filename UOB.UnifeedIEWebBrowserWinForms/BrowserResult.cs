namespace UOL.UnifeedIEWebBrowserWinForms
{
	internal enum BrowserResultType
	{
		UnknownError,
		Success,
		Timeout
	}

	internal class BrowserResult
	{
		public string Response { get; set; }

		public BrowserResultType ResultType { get; set; }

		public string Error { get; internal set; }
	}
}