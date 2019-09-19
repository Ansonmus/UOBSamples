namespace UOL.SharedCode.Web
{
	using System;
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;

	public class LoopbackHttpListener : IDisposable
	{
		private const int DefaultTimeout = 60 * 5; // 5 mins (in seconds)

		private HttpListener _listener;
		private TaskCompletionSource<string> _source = new TaskCompletionSource<string>();
		private string _listenerPrefix;

		public LoopbackHttpListener(string listenerPrefix)
		{
			_listenerPrefix = listenerPrefix;
		}

		public async void Start()
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add(_listenerPrefix);
			_listener.Start();

			// wait for the authorization response.
			var context = await _listener.GetContextAsync().ConfigureAwait(false);
			SetResult(context.Request.Url.ToString(), context);
		}

		public void Dispose()
		{
			Task.Run(async () =>
			{
				await Task.Delay(500);
				_listener.Close();
				//_host.Dispose();
			});
		}

		internal Task<string> WaitForCallbackAsync(int timeoutInSeconds = DefaultTimeout)
		{
			Task.Run(async () =>
			{
				await Task.Delay(timeoutInSeconds * 1000);
				_source.TrySetCanceled();
			});

			return _source.Task;
		}

		private void SetResult(string value, HttpListenerContext ctx)
		{
			try
			{
				ctx.Response.StatusCode = 200;
				ctx.Response.ContentType = "text/html";
				string responseString = string.Format("<html><head><meta http-equiv='refresh' content='20;url=https://www.2ba.nl/uob'></head><body><strong>Authenticatie geslaagd. U kunt dit scherm sluiten en terugkeren naar de applicatie!</strong></body></html>");
				var buffer = Encoding.UTF8.GetBytes(responseString);
				ctx.Response.ContentLength64 = buffer.Length;
				var responseOutput = ctx.Response.OutputStream;
				responseOutput.Write(buffer, 0, buffer.Length);
				responseOutput.Close();

				_source.TrySetResult(value);
			}
			catch
			{
				ctx.Response.StatusCode = 400;
				ctx.Response.ContentType = "text/html";
				string responseString = string.Format("<h1>Invalid request.</h1>");
				var buffer = Encoding.UTF8.GetBytes(responseString);
				ctx.Response.ContentLength64 = buffer.Length;
				var responseOutput = ctx.Response.OutputStream;
				responseOutput.Write(buffer, 0, buffer.Length);
				responseOutput.Close();
			}
		}
	}
}