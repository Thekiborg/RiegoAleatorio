using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace RiegoAutomatico
{
	internal static class ExcelFileWriter
	{
		/// <summary>
		/// Escribe la información de los días en un archivo nuevo de excel.<br></br>
		/// El archivo se guardará con el nombre prevision_semana.xlsx, y tendrá una hoja llamada 'Prevision riego proxima semana'<br></br>
		/// La hoja tendrá tantas filas como días tenga la lista proporcionada.
		/// </summary>
		/// <param name="data">Lista con los datos de cada día.</param>
		/// <param name="path">Ruta donde se guardará el archivo.</param>
		internal static void WriteWeatherDataToXLSX(List<WeatherDayData> data, string path)
		{
			// XSSFWorkbook es un libro de excel, el contenido del archivo.
			// El archivo como tal se crea cuando se escribe con el FileStream.
			var excelBook = new XSSFWorkbook();

			var sheet = excelBook.CreateSheet("Prevision riego proxima semana");

			// Crear los titulos la primera fila
			CreateHeaders(sheet);

			// Llenar las filas con los datos.
			for (int line = 0; line < data.Count; line++)
			{
				WeatherDayData day = data[line];

				// Se crea una fila en el indice + 1 para no sobreescribir los titulos.
				var row = sheet.CreateRow(line + 1);

				// Se van creando celdas en la fila creada con los datos.
				row.CreateCell(0).SetCellValue(day.Date);
				row.CreateCell(1).SetCellValue(day.Precipitation);
				row.CreateCell(2).SetCellValue(day.NeedsIrrigation);

				if (day.NeedsIrrigation)
					row.CreateCell(3).SetCellValue(day.IrrigationAmount);
				else
					row.CreateCell(3).SetCellValue("null");
			}

			try
			{
				// Escribimos los datos del libro en un archivo.
				using var fileStream = File.Create(path);
				excelBook.Write(fileStream);
			}
			catch (Exception ex)
			{
				Console.WriteLine("No se han podido guardar los datos en " + path + ": " + ex.Message);
			}
		}


		/// <summary>
		/// Pone títulos en las columnas de la primera fila de la hoja.<br></br>
		/// Para no sobreescribir, se debe empezar a escribir la información desde la segunda fila.
		/// </summary>
		/// <param name="sheet">La hoja dónde escribir los títulos.</param>
		private static void CreateHeaders(ISheet sheet)
		{
			var row = sheet.CreateRow(0);
			row.CreateCell(0).SetCellValue("Dia");
			row.CreateCell(1).SetCellValue("Precipitacion");
			row.CreateCell(2).SetCellValue("Necesita riego");
			row.CreateCell(3).SetCellValue("Riego necesario");
		}
	}
}
