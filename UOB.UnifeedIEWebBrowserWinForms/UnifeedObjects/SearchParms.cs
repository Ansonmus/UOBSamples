namespace UOL.UnifeedIEWebBrowserWinForms.UnifeedObjects
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public class SearchParms
	{
		public int From { get; set; } = 0;

		public int Size { get; set; } = 20;

		public string Languagecode { get; set; }

		public string SearchString { get; set; }

		public IEnumerable<FilterModel> Filters { get; set; }

		public IEnumerable<FilterFeatureModel> FeatureFilters { get; set; }
	}
}
