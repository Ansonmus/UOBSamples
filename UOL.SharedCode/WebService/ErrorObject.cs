namespace UOL.SharedCode.WebService
{
	using Newtonsoft.Json;

	internal class ErrorObject
	{
		#region Fields
		// Disable warning 649 as private fields are filled by JSON deserialization
#pragma warning disable 649
		[JsonProperty("IsError")]
		private bool isError;

		[JsonProperty("ErrorMessage")]
		private string errorMessage;

		[JsonProperty("Status")]
		private string status;
#pragma warning restore 649
		#endregion

		#region Properties
		public bool IsError
		{
			get { return this.isError; }
		}

		public string ErrorMessage
		{
			get { return this.errorMessage; }
		}

		public string Status
		{
			get { return this.status; }
		}
		#endregion
	}
}
