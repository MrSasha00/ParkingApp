using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ParkingApp.Models;
using ParkingApp.Pages;
using ParkingApp.Services;
using Xamarin.Forms;

namespace ParkingApp.ViewModel
{
	/// <summary>
	/// Вспомогателная модель для отображения данных.
	/// </summary>
	public class ApplicationViewModel : INotifyPropertyChanged
	{
		#region Fields/Private

		/// <summary>
		/// Идентификатор выбранной парковки.
		/// </summary>
		private int _selectedId;

		/// <summary>
		/// Сервис для работы с API.
		/// </summary>
		private readonly ParkingService _parkingService;

		/// <summary>
		/// Выбранная парковка.
		/// </summary>
		private readonly Parking selectedParking;

		/// <summary>
		/// Выбранная парковка.
		/// </summary>
		private DetailParking _selectedDetailParking;

		/// <summary>
		/// Для навигации.
		/// </summary>
		private INavigation _navigation;

		/// <summary>
		/// Флаг загрузки.
		/// </summary>
		private bool _isRefreshing;

		#endregion

		#region Fields/Public

		/// <summary>
		/// Список парковок. 
		/// </summary>
		public ObservableCollection<Parking> ParkingPlaces { get; set; }

		/// <summary>
		/// Выбранная парковка.
		/// </summary>
		public DetailParking SelectedDetailParking
		{
			get => _selectedDetailParking;
			private set => _selectedDetailParking = value;
		}

		/// <summary>
		/// Для навигации между страницами.
		/// </summary>
		public INavigation Navigation
		{
			get => _navigation;
			set => _navigation = value;
		}

		/// <summary>
		/// Публичное поле для выбранной парковки.
		/// </summary>
		public Parking SelectedParking
		{
			get => selectedParking;
			set
			{
				if (selectedParking != value)
				{
					_selectedId = value.Id;
					var tempParking = new Parking
					{
						Id = value.Id,
						Address = value.Address,
						FreeParkingSpaces = value.FreeParkingSpaces,
						TotalParkingSpaces = value.TotalParkingSpaces
					};
					_selectedDetailParking = null;
					NotifyPropertyChanged("SelectedParking");
					Navigation.PushAsync(new ParkingPage(this));
				}
			}
		}

		/// <summary>
		/// Команда для обновления.
		/// </summary>
		public ICommand RefreshCommand
		{
			get {
				return new Command(async () =>
				{
					IsRefreshing = true;
					await GetParkingPlaces();
					IsRefreshing = false;
				});
			}
		}

		/// <summary>
		/// Флаг загрузки.
		/// </summary>
		public bool IsRefreshing 
		{ 
			get { return _isRefreshing; }
			set {
				_isRefreshing = value;
				NotifyPropertyChanged("IsRefreshing");
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Событие для оповещения об изменениях состояния.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Оповещает об изменениях.
		/// </summary>
		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Конструктор.
		/// </summary>
		public ApplicationViewModel()
		{
			ParkingPlaces = new ObservableCollection<Parking>();
			_parkingService = new ParkingService();
		}

		/// <summary>
		/// Получает все парковки.
		/// </summary>
		public async Task GetParkingPlaces()
		{
			RemoveItems();
			var parkingPlaces = await _parkingService.GetAll();
			foreach (var parking in parkingPlaces)
			{
				ParkingPlaces.Add(parking);
			}
		}

		/// <summary>
		/// Получение парковки по Id.
		/// </summary>
		public async Task GetParkById()
		{
			IsRefreshing = true;
			SelectedDetailParking = await _parkingService.Get(_selectedId);
			NotifyPropertyChanged("SelectedDetailParking");
			IsRefreshing = false;
		}

		/// <summary>
		/// Удаляет все элементы из списка.
		/// </summary>
		private void RemoveItems()
		{
			foreach (var parkingPlace in ParkingPlaces.ToList())
			{
				ParkingPlaces.Remove(parkingPlace);
			}
		}

		#endregion
	}
}