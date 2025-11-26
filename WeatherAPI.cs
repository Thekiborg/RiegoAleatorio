namespace RiegoAutomatico
{
	internal static class WeatherAPI
	{
		private static readonly HttpClient httpClient = new();
		private const string URL = @"https://api.open-meteo.com/v1/forecast?latitude=40&longitude=-4&daily=precipitation_sum&timezone=Europe%2FLondon&past_days=1";


		/// <summary>
		/// Intenta llamar a la api de open-meteo para que devuelva la media de precipitaciones de los proximos 7 dias.
		/// </summary>
		/// <returns>Un JSON con la información</returns>
		internal static async Task<string?> GetWeatherData()
		{
			try
			{
				var response = await httpClient.GetAsync(URL);

				return await response.Content.ReadAsStringAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine("No se ha podido llamar a la API: " + ex.Message);
				return null;
			}
		}
	}
}
