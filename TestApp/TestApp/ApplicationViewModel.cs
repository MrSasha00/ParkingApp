using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TestApp.Annotations;

namespace TestApp
{
	/// <summary>
	/// Вспомогателная модель для отображения данных.
	/// </summary>
	public class ApplicationViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Событие для оповещения об изменениях состояния.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Список парковок. 
		/// </summary>
		public ObservableCollection<Parking> Parkings { get; set; }

		/// <summary>
		/// Сервис для работы с API.
		/// </summary>
		private ParkingService _parkingService;

		/// <summary>
		/// Конструктор.
		/// </summary>
		public ApplicationViewModel()
		{
			Parkings = new ObservableCollection<Parking>();
			_parkingService = new ParkingService();
			GetParkings();
		}

		/// <summary>
		/// Получает все парковки.
		/// </summary>
		public async void GetParkings()
		{
			var parkings = await _parkingService.Get();
			foreach (var parking in parkings)
			{
				Parkings.Add(parking);
			}
		}

		/// <summary>
		/// Оповещает об изменениях состояния.
		/// </summary>
		/// <param name="propertyName"></param>
		[NotifyPropertyChangedInvocator]
		protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}