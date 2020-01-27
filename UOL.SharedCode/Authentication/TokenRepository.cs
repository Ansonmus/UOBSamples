namespace UOL.SharedCode.Authentication
{
	using System;
	using Microsoft.Win32;

	public static class TokenRepository
	{
		private const string Path = @"SOFTWARE\2BA UOB SampleTool";

		public static void StoreToken(OAuthToken currentToken)
		{
			var key = Registry.CurrentUser.CreateSubKey(Path);
			var tokenString = Newtonsoft.Json.JsonConvert.SerializeObject(currentToken);
			key.SetValue("LastToken", tokenString);
			key.Close();
		}

		public static OAuthToken GetToken()
		{
			using (var key = Registry.CurrentUser.OpenSubKey(Path))
			{
				if (key != null)
				{
					var o = key.GetValue("LastToken");
					if (o != null)
					{
						return Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthToken>(o.ToString());
					}
				}
			}

			return null;
		}
	}
}
