namespace ParkingApp.Models
{
	/// <summary>
	/// Модель для получения информации о парковках.
	/// </summary>
	public class Parking
	{
		/// <summary>
		/// Идентификатор.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Адрес.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Всего парковочных мест.
		/// </summary>
		public int TotalParkingSpaces { get; set; }

		/// <summary>
		/// Свободных парковочных мест.
		/// </summary>
		public int FreeParkingSpaces { get; set; }
	}
}