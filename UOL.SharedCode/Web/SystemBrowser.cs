namespace UOL.SharedCode.Web
{
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Net.Sockets;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;

	public class SystemBrowser
	{
		private readonly string _listenerPrefix;

		public SystemBrowser(string listenerPrefix = "")
		{
			if (string.IsNullOrWhiteSpace(listenerPrefix))
			{
				listenerPrefix = $"http://localhost:{GetRandomUnusedPort()}/";
			}

			_listenerPrefix = listenerPrefix;
		}

		private int GetRandomUnusedPort()
		{
			var listener = new TcpListener(IPAddress.Loopback, 0);
			listener.Start();
			var port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
			return port;
		}

		public async Task<BrowserResult> InvokeAsync(string url)
		{
			using (var listener = new LoopbackHttpListener(_listenerPrefix))
			{
				listener.Start();
				OpenBrowser(url);

				try
				{
					var result = await listener.WaitForCallbackAsync();
					if (string.IsNullOrWhiteSpace(result))
					{
						return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." };
					}

					return new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
				}
				catch (TaskCanceledException ex)
				{
					return new BrowserResult { ResultType = BrowserResultType.Timeout, Error = ex.Message };
				}
				catch (Exception ex)
				{
					return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
				}
			}
		}

		internal static void OpenBrowser(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch
			{
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start("xdg-open", url);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					Process.Start("open", url);
				}
				else
				{
					throw;
				}
			}
		}
	}
}