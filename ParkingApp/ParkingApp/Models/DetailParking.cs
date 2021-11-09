namespace ParkingApp.Models
{
	/// <summary>
	/// Модель для получения детальной информации о парковке.
	/// </summary>
	public class DetailParking : Parking
	{
		/// <summary>
		/// Координата X.
		/// </summary>
		public double CoordX { get; set; }

		/// <summary>
		/// Координата Y.
		/// </summary>
		public double CoordY { get; set; }

		/// <summary>
		/// Ссылка на камеру.
		/// </summary>
		public string Camera { get; set; }
	}
}