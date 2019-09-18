namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using System.Text;

	public class PKCECode
	{
		public string CodeChallenge { get; set; }

		public string CodeVerifier { get; set; }

		public static PKCECode GeneratePKCECodes()
		{
			var bytes = new byte[32];
			using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
			{
				rng.GetBytes(bytes);
			}

			// It's recommended that the code_verifier be a URL-safe string
			// See the Section 4 of the RFC 7636 for more details.
			var code_verifier = Convert.ToBase64String(bytes)
				.TrimEnd('=')
				.Replace('+', '-')
				.Replace('/', '_');

			var code_challenge = string.Empty;
			using (var sha256 = System.Security.Cryptography.SHA256.Create())
			{
				var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(code_verifier));
				code_challenge = Convert.ToBase64String(challengeBytes)
					.TrimEnd('=')
					.Replace('+', '-')
					.Replace('/', '_');
			}

			return new PKCECode()
			{
				CodeVerifier = code_verifier,
				CodeChallenge = code_challenge,
			};
		}
	}
}
