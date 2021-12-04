using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ParkingApp.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ParkingApp.Services
{
	/// <summary>
	/// Сервис для работы с API.
	/// </summary>
	public class ParkingService
	{
		/// <summary>
		/// Текстовая строка подключения.
		/// </summary>
		private static readonly string connectionString = "http://89.108.88.254:81";

		/// <summary>
		/// Строка подключения.
		/// </summary>
		private readonly Uri _url = new Uri("http://89.108.88.254:81/api/v1/");

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
			var httpResponse = client.GetAsync("parkings/");
			using (var response = await httpResponse)
			{
				if (response.IsSuccessStatusCode)
				{
					var contentSting = await response.Content.ReadAsStringAsync();
					return JsonSerializer.Deserialize<IEnumerable<Parking>>(contentSting, _options);
				}
			}
			return new List<Parking>();
		}

		/// <summary>
		/// Получает детальную информацию о праковке.
		/// </summary>
		/// <param name="id">Идентификатор парковки.</param>
		/// <returns>Детальная информация о парковке.</returns>
		public async Task<DetailParking> Get(int id)
		{
			var client = GetClient();
			var httpResponse = client.GetAsync("parkings/" + id);
			using (var response = await httpResponse)
			{
				if (response.IsSuccessStatusCode)
				{
					var contentSting = await response.Content.ReadAsStringAsync();
					var detailParking = JsonSerializer.Deserialize<DetailParking>(contentSting, _options);
					if (detailParking != null)
					{
						detailParking.Camera = _url + "parkings/" + id + "/camera";
						return detailParking;
					}
				}
			}
			throw new NullReferenceException();
		}

		/// <summary>
		/// Полкчает фото по ссылке.
		/// </summary>
		/// <param name="url">Адрес фотографии.</param>
		/// <returns>Фото.</returns>
		public async Task<byte[]> GetPhoto(string url)
		{
			var client = GetClient();
			return await client.GetByteArrayAsync(url);
		}
	}
}