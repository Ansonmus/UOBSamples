namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using CefSharp;

	public class CustomSchemeHandlerFactory : ISchemeHandlerFactory
	{
		public CustomSchemeHandlerFactory(string schemeName)
		{
			this.SchemeName = schemeName;
		}

		public event EventHandler<UnifeedSchemaHandlerEventArgs> CustomSchemeTriggered;

		public string SchemeName { get; }

		public static void RegisterUnifeedSchemaHandler(string schemeName, EventHandler<UnifeedSchemaHandlerEventArgs> eh)
		{
			var factory = new CustomSchemeHandlerFactory(schemeName);
			factory.CustomSchemeTriggered += eh;
			var settings = new CefSharp.WinForms.CefSettings();
			settings.RegisterScheme(new CefCustomScheme()
			{
				SchemeName = factory.SchemeName,
				SchemeHandlerFactory = factory
			});

			Cef.Initialize(settings);
		}

		public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
		{
			if (schemeName == SchemeName)
			{
				CustomSchemeTriggered?.Invoke(this, new UnifeedSchemaHandlerEventArgs(request));
			}

			return new ResourceHandler();
		}
	}
}