namespace UOL.UnifeedXIEWebBrowserWinForms.UnifeedObjects
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public class FilterFeatureModel
	{
		// [Required]
		// [StringLength(8)]
		public string Code { get; set; }

		public IEnumerable<FilterValueModel> Values { get; set; }
	}
}
