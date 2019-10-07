namespace UOL.SharedCode.WebService
{
	using Newtonsoft.Json;

	internal class ErrorObject
	{
		#region Fields
		// Disable warning 649 as private fields are filled by JSON deserialization
#pragma warning disable 649
		[JsonProperty("IsError")]
#pragma warning disable IDE0044 // Add readonly modifier
		private bool _isError;
#pragma warning restore IDE0044 // Add readonly modifier

		[JsonProperty("ErrorMessage")]
#pragma warning disable IDE0044 // Add readonly modifier
		private string _errorMessage;
#pragma warning restore IDE0044 // Add readonly modifier

		[JsonProperty("Status")]
#pragma warning disable IDE0044 // Add readonly modifier
		private string _status;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore 649
		#endregion

		#region Properties
		public bool IsError
		{
			get { return this._isError; }
		}

		public string ErrorMessage
		{
			get { return this._errorMessage; }
		}

		public string Status
		{
			get { return this._status; }
		}
		#endregion
	}
}
