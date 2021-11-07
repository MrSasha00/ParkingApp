using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp
{
	/// <summary>
	/// Сервис для работы с API.
	/// </summary>
	public class ParkingService
	{
		/// <summary>
		/// Строка подключения.
		/// </summary>
		private Uri Url = new Uri("http://167.172.39.93/api/v1/");

		/// <summary>
		/// Настройка сериализатора.
		/// </summary>
		JsonSerializerOptions options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
		};

		/// <summary>
		/// Настраивает http клиент.
		/// </summary>
		/// <returns></returns>
		private HttpClient GetClient()
		{
			var client = new HttpClient {BaseAddress = Url};
			client.DefaultRequestHeaders.Add("Accept", "application/json");
			return client;
		}

		/// <summary>
		/// Получает все парковки.
		/// </summary>
		/// <returns>Список парковок.</returns>
		public async Task<IEnumerable<Parking>> GetAll()
		{
			var client = GetClient();
			var response = await client.GetStringAsync("parkings/");
			var res = JsonSerializer.Deserialize<IEnumerable<Parking>>(response, options);
			return res;
		}

		/// <summary>
		/// Получает детальную информацию о праковке.
		/// </summary>
		/// <returns>Детальная информация о парковке.</returns>
		public async Task<DetailParking> Get(int id)
		{
			var client = GetClient();
			var response = await client.GetStringAsync("parkings/" + id);
			var res = JsonSerializer.Deserialize<DetailParking>(response, options);
			return res;
		}
	}
}