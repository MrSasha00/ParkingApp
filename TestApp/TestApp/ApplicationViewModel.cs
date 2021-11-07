using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Annotations;
using Xamarin.Forms;

namespace TestApp
{
	/// <summary>
	/// Вспомогателная модель для отображения данных.
	/// </summary>
	public class ApplicationViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Флаг инициализации.
		/// </summary>
		private bool IsInit = false;
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
		/// Выбранная парковка.
		/// </summary>
		private Parking selectedParking;

		/// <summary>
		/// Для навигации между страницами.
		/// </summary>
		public INavigation Navigation { get; set; }

		/// <summary>
		/// Команда назад.
		/// </summary>
		public ICommand BackCommand { protected set; get; }

		/// <summary>
		/// Конструктор.
		/// </summary>
		public ApplicationViewModel()
		{
			Parkings = new ObservableCollection<Parking>();
			_parkingService = new ParkingService();
			BackCommand = new Command(Back);
		}

		public Parking SelectedParking
		{
			get => selectedParking;
			set
			{
				if (selectedParking != value)
				{
					var tempParking = new Parking
					{
						Id = value.Id,
						Address = value.Address,
						FreeParkingSpaces = value.FreeParkingSpaces,
						TotalParkingSpaces = value.TotalParkingSpaces
					};
					selectedParking = null;
					NotifyPropertyChanged("SelectedParking");
					// TODO: Передать сюда страницу с детальной парковкой
					Navigation.PushAsync(new ParkingPage(tempParking, this));
				}
			}
		}

		/// <summary>
		/// Получает все парковки.
		/// </summary>
		public async Task GetParkings()
		{
			if (IsInit)
				return;
			var parkings = await _parkingService.GetAll();
			foreach (var parking in parkings)
			{
				Parkings.Add(parking);
			}

			IsInit = true;
		}

		/// <summary>
		/// Оповещает об изменениях.
		/// </summary>
		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Для навигации назад.
		/// </summary>
		private void Back()
		{
			Navigation.PopAsync();
		}
	}
}