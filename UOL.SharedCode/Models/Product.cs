namespace UOL.SharedCode.Models
{
	using System;
	using System.Collections.Generic;

	public class Product
	{
		public long? Id { get; set; }

		public string ManufacturerGln { get; set; }

		public string ManufacturerName { get; set; }

		public string ProductCode { get; set; }

		public string Gtin { get; set; }

		public string Deeplink { get; set; }

		public MLDescription Brand { get; set; }

		public MLDescription Model { get; set; }

		public MLDescription Version { get; set; }

		public ProductClass ProductClass { get; set; }

		public ProductClass ModellingClass { get; set; }

		public string StatusCode { get; set; }

		public decimal? WeightQuantity { get; set; }

		public string WeightMeasureUnitCode { get; set; }

		public string WeightMeasureUnitDescription { get; set; }

		public string SuccessorGtin { get; set; }

		public string SuccessorProductCode { get; set; }

		public string PredecessorGtin { get; set; }

		public string PredecessorProductCode { get; set; }

		public IEnumerable<Feature> Features { get; set; }

		// public DateTime? ChangeDate { get; set; } // Temporary a string until we have a solution on the WCF application to receive nullable datetimes.
		public string ChangeDate { get; set; }

		// public DateTime? StartDate { get; set; } // Temporary a string until we have a solution on the WCF application to receive nullable datetimes.
		public string StartDate { get; set; }

		public bool IsDummy { get; set; }
	}
}