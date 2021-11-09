using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ParkingApp.Models;

namespace ParkingApp.Services
{
	/// <summary>
	/// Сервис для работы с API.
	/// </summary>
	public class ParkingService
	{
		/// <summary>
		/// Строка подключения.
		/// </summary>
		private readonly Uri _url = new Uri("http://167.172.39.93/api/v1/");

		/// <summary>
		/// Настройка сериализатора.
		/// </summary>
		readonly JsonSerializerOptions _options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
		};

		/// <summary>
		/// Настраивает http клиент.
		/// </summary>
		/// <returns>Http-клиент</returns>
		private HttpClient GetClient()
		{
			var client = new HttpClient {BaseAddress = _url};
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
			return JsonSerializer.Deserialize<IEnumerable<Parking>>(response, _options);
		}

		/// <summary>
		/// Получает детальную информацию о праковке.
		/// </summary>
		/// <returns>Детальная информация о парковке.</returns>
		public async Task<DetailParking> Get(int id)
		{
			var client = GetClient();
			var response = await client.GetStringAsync("parkings/" + id);
			return JsonSerializer.Deserialize<DetailParking>(response, _options);
		}
	}
}