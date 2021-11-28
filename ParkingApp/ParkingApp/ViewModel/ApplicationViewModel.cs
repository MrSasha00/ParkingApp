using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
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
		/// Хаб webSoket.
		/// </summary>
		private HubConnection _hubConnection;

		/// <summary>
		/// Список парковок.
		/// </summary>
		private ObservableCollection<Parking> _parkingPlaces;

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
		public ObservableCollection<Parking> ParkingPlaces
		{ get; set; }

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
					Sorting();
					IsRefreshing = false;
					NotifyPropertyChanged("RefreshCommand");
				});
			}
		}

		/// <summary>
		/// Команда для поиска в коллекци.
		/// </summary>
		public ICommand PerformSearch => new Command<string>(SelectByAddress);

		/// <summary>
		/// Флаг загрузки.
		/// </summary>
		public bool IsRefreshing
		{
			get { return _isRefreshing; }
			set
			{
				_isRefreshing = value;
				NotifyPropertyChanged("IsRefreshing");
			}
		}

		/// <summary>
		/// Фото выбранной парковки.
		/// </summary>
		public byte[] Photo { get; set; }

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
			_parkingPlaces = new ObservableCollection<Parking>();
			_parkingService = new ParkingService();
			ParkingPlaces = new ObservableCollection<Parking>();
			// TODO: добавить url.
			_hubConnection = new HubConnectionBuilder()
				.WithUrl("URL")
				.Build();
			_hubConnection.On<int, int>("Receive", UpdateByHub);
		}

		/// <summary>
		/// Получает все парковки.
		/// </summary>
		public async Task GetParkingPlaces()
		{
			_parkingPlaces.Clear();
			ParkingPlaces.Clear();
			var parkingPlaces = await _parkingService.GetAll();
			foreach (var parking in parkingPlaces)
			{
				_parkingPlaces.Add(parking);
				ParkingPlaces.Add(parking);
			}
			Sorting();
			NotifyPropertyChanged("Added");
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
		/// Фильтрует коллекцию по входящей строке.
		/// </summary>
		/// <param name="query"></param>
		public void SelectByAddress(string query)
		{
			ParkingPlaces.Clear();
			var res = _parkingPlaces.ToList().Where(x => x.Address.ToLower().Contains(query.ToLower()));
			foreach (var parking in res)
			{
				ParkingPlaces.Add(parking);
			}
		}

		/// <summary>
		/// Получает фото.
		/// </summary>
		public async Task UpdatePhoto()
		{
			Photo = await _parkingService.GetPhoto(_selectedDetailParking.Camera);
			NotifyPropertyChanged("PhotoUpdated");
		}

		/// <summary>
		/// Сортирует список.
		/// </summary>
		public void Sorting()
		{
			var sortableList = new List<Parking>(ParkingPlaces);
			var orderedEnumerable = sortableList.OrderBy(x => x.Address).ToList();
			ParkingPlaces.Clear();
			foreach (var parkingPlace in orderedEnumerable)
			{
				ParkingPlaces.Add(parkingPlace);
			}
			NotifyPropertyChanged("ListSorted");
		}

		/// <summary>
		/// Обновляет данные в списке.
		/// </summary>
		public async Task UpdateList()
		{
			var updatedList = (List<Parking>)await _parkingService.GetAll();
			foreach (var updatedParking in updatedList)
			{
				foreach (var parkingPlace in ParkingPlaces)
				{
					if (updatedParking.Id == parkingPlace.Id)
					{
						parkingPlace.FreeParkingSpaces = updatedParking.FreeParkingSpaces;
					}
				}
			}
			Sorting();
			NotifyPropertyChanged("ListUpdated");
		}

		private void UpdateByHub(int id, int newFreeSpaces)
		{
			if (id == _selectedDetailParking?.Id && _selectedDetailParking != null)
			{
				_selectedDetailParking.FreeParkingSpaces = newFreeSpaces;
				Notify?.Invoke(newFreeSpaces);
			}
		}

		public delegate void AccountHandler(int message);
		public event AccountHandler Notify;  
		
		#endregion
		
	}
}