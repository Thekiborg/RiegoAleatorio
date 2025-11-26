using System.Text;

namespace RiegoAutomatico
{
	internal class WeatherDayData
	{
		private readonly string date;
		private readonly float precipitation;
		private const float IrrigationThreshold = 7f;


		public string Date => date;
		public float Precipitation => precipitation;
		public bool NeedsIrrigation => Precipitation < IrrigationThreshold;
		public float IrrigationAmount
		{
			get
			{
				if (!NeedsIrrigation)
				{
					return 0;
				}
				else
				{
					return IrrigationThreshold - Precipitation;
				}
			}
		}


		public WeatherDayData(string date, float precipitation)
		{
			this.date = date;
			this.precipitation = precipitation;
		}


		public override string ToString()
		{
			StringBuilder sb = new();
			sb.Append(DateOnly.Parse(date));
			sb.Append($" - {Precipitation}mm");
			if (NeedsIrrigation)
			{
				sb.Append($" - Sí");
			}
			else
			{
				sb.Append($" - No");
			}
			sb.Append($" - {IrrigationAmount}mm");
			return sb.ToString();
		}
	}
}
