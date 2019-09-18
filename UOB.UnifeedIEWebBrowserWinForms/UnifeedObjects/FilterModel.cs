namespace UOL.UnifeedIEWebBrowserWinForms.UnifeedObjects
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public class FilterModel
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public Filtercode Code { get; set; }

		public IEnumerable<FilterValueModel> Values { get; set; }
	}
}
