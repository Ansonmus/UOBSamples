namespace UOL.SharedCode.Models
{
	public class Feature
	{
		public string Code { get; set; }

		public string Description { get; set; }

		public Value Value { get; set; }

		public Unit Unit { get; set; }

		public Unit UnitVariableAxis { get; set; }

		public decimal? NumericValue { get; set; }

		public decimal? RangeLowerValue { get; set; }

		public decimal? RangeUpperValue { get; set; }

		public bool? LogicalValue { get; set; }

		public string Type { get; set; }

		public int OrderNumber { get; set; }

		public int PortCode { get; set; }

		public decimal? XCoordinate { get; set; }

		public decimal? YCoordinate { get; set; }

		public decimal? ZCoordinate { get; set; }

		public MatrixValue[] MatrixValues { get; set; }

		public string DimensionalDrawingCode { get; set; }
	}
}