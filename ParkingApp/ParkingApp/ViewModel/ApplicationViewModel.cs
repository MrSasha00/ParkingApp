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
using System;

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

		/// <summary>
		/// Флаг подключения.
		/// </summary>
		bool isConnected;

		#endregion

		#region Fields/Public

		/// <summary>
		/// Делегат для события обновления.
		/// </summary>
		public delegate void UpdateParking(int message);

		/// <summary>
		/// Событие обновления.
		/// </summary>
		public event UpdateParking Notify;  

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

		/// </summary>
		///Флаг подключение (может как-то будем обрабатывать)
		/// </summary>
		public bool IsConnected
		{
			get => isConnected;
			set
			{
				if (isConnected != value)
				{
					isConnected = value;
					//NotifyPropertyChanged("IsConnected");
				}
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
			_parkingPlaces = new ObservableCollection<Parking>();
			_parkingService = new ParkingService();
			ParkingPlaces = new ObservableCollection<Parking>();

			_hubConnection = new HubConnectionBuilder()
				.WithUrl("http://89.108.88.254:81/connect")
				.Build();

			//Пытаемся переподключиться в случае сброса соединения 
			_hubConnection.Closed += async (error) =>
			{
				IsConnected = false;
				await Task.Delay(5000);
				await Connect();
			};

			//Прием новой информации
			_hubConnection.On<int, int>("Notify", UpdateByHub);
		}
		/// <summary>
		/// Подключение к серверу
		/// </summary>
		public async Task Connect()
		{
			if (IsConnected)
				return;
			try
			{
				await _hubConnection.StartAsync();
				IsConnected = true;
			}
			catch (Exception ex)
			{
				//Можно какой-нибудь обработчик придумать
				//SendLocalMessage(String.Empty, $"Ошибка подключения: {ex.Message}");
			}
		}
		/// </summary>
		///Отключение от сервера
		/// </summary>
		public async Task Disconnect()
		{
			if (!IsConnected)
				return;

			await _hubConnection.StopAsync();
			IsConnected = false;
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
			var orderedEnumerable = sortableList.OrderBy(x => x.Id).ToList();
			ParkingPlaces.Clear();
			foreach (var parkingPlace in orderedEnumerable)
			{
				ParkingPlaces.Add(parkingPlace);
			}
			NotifyPropertyChanged("ListSorted");
		}

		/// <summary>
		/// Обновление по уведомлениб от сервера.
		/// </summary>
		/// <param name="parkingId">Идентфиикатор парковки.</param>
		/// <param name="newFreeSpaces">Новое количество свободных парковочных мест.</param>
		private void UpdateByHub(int parkingId, int newFreeSpaces)
		{
			//Обновляем значение у данной парковки в общем списке
			foreach (var parkingPlace in ParkingPlaces)
			{
				if (parkingId == parkingPlace.Id)
				{
					parkingPlace.FreeParkingSpaces = newFreeSpaces;
				}
			}
			//Если праковка не выбрана или Id не совпадает, значит мы в главном меню и обновляем его
			if (_selectedDetailParking == null || parkingId != _selectedDetailParking.Id)
			{
				Sorting();
				NotifyPropertyChanged("ListUpdated");
			}
			//Иначе обновляем страницу с детальной парковкой и картинку
 			else
 			{ 
				_selectedDetailParking.FreeParkingSpaces = newFreeSpaces;
				NotifyPropertyChanged("SelectedDetailParking");
				Notify?.Invoke(newFreeSpaces);
			}
		}

		#endregion
	}
}