namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using CefSharp;

	public class UnifeedSchemaHandlerEventArgs : EventArgs
	{
		public UnifeedSchemaHandlerEventArgs(IRequest request)
		{
			this.Request = request;
		}

		public IRequest Request { get; }
	}
}
