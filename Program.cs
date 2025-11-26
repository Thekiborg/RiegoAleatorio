using System.Text.Json;
using System.Globalization;

namespace RiegoAutomatico
{
    internal class Program
    {
		private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private static string FullPath => $@"{DesktopPath}\prevision_semana.xlsx";


		static async Task Main(string[] args)
        {
            string? json = await WeatherAPI.GetWeatherData();
            
            if (json is not null)
            {
                try
                {
					var data = ReadJson(json);
					ShowDataInConsole(data);
					ExcelFileWriter.WriteWeatherDataToXLSX(data, FullPath);
					// Los datos se guardarán en el escritorio bajo el nombre prevision_semana.xlsx.
				}
				catch (Exception ex)
                {
                    Console.WriteLine("Error al leer los datos de la API: " + ex.Message);
                }
			}

			Thread.Sleep(-1);
            // Evita que la consola se cierre.
        }


        /// <summary>
        /// Muestra por consola la información de cada día.
        /// </summary>
        /// <param name="data">La lista de días que mostrar.</param>
        private static void ShowDataInConsole(List<WeatherDayData> data)
        {
            Console.WriteLine("Dia - Precipitación - Necesita riego - Riego necesario");
            foreach (WeatherDayData day in data)
            {
                Console.WriteLine(day.ToString());
            }
            Console.WriteLine("Guardando datos en " + FullPath);
        }


        /// <summary>
        /// Convierte el JSON leído en WeatherDayData.<br></br>
        /// WeatherDayData guarda la fehca, la precipitación y calcula si necesita riego.
        /// </summary>
        /// <param name="rawJson">El JSON devuelto por la API</param>
        /// <returns>Una lista con tantos WeatherDayData como días devuelva la API</returns>
        /// <exception cref="InvalidDataException">Si cualquiera de los datos esperados no tuviera un valor.</exception>
        private static List<WeatherDayData> ReadJson(string rawJson)
        {
            var root = JsonDocument.Parse(rawJson).RootElement;

            var found = root.TryGetProperty("error", out _);
			if (found)
            {
                throw new InvalidDataException("Error devuelto por la API: " + root.GetProperty("reason").GetRawText());
            }
            else
            {
                bool foundDayRainData = root.TryGetProperty("daily", out var dayRainData);
                if (!foundDayRainData)
                {
					throw new InvalidDataException("La API no tiene datos de días.");
				}

				bool foundDays = dayRainData.TryGetProperty("time", out var days);
                if (!foundDays)
                {
					throw new InvalidDataException("La API no tiene una lista de fechas.");
				}

                bool foundRain = dayRainData.TryGetProperty("precipitation_sum", out var rain);
				if (!foundRain)
                {
                    throw new InvalidDataException("La API no tiene información sobre precipitaciones.");
                }

				List<WeatherDayData> weatherDayDatas = [];
				for (int i = 0; i < days.GetArrayLength(); i++)
				{
					string date = days[i].ToString();
                    float rainFloat;
                    try
                    {
						rainFloat = float.Parse(rain[i].ToString(), CultureInfo.InvariantCulture);
					}
					catch
                    {
						throw new InvalidDataException("Los datos de precipitaciones de la API no están en el formato esperado.");
					}
					weatherDayDatas.Add(new(date, rainFloat));
				}

				return weatherDayDatas;
			}
		}
	}
}
