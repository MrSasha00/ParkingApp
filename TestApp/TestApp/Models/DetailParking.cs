using System;

namespace TestApp.Models
{
	public class DetailParking : Parking
	{
		public double CoordX { get; set; }
		public double CoordY { get; set; }
		public string CameraLink { get; set; }
	}
}