namespace UOL.UnifeedXIEWebBrowserWinForms.UnifeedObjects
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public class FilterValueModel
	{
		public string Code { get; set; }

		public double? Min { get; set; }

		public double? Max { get; set; }

		public double? Range { get; set; }
	}
}
