using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Models;
using Xamarin.Forms;

namespace TestApp
{
	/// <summary>
	/// Вспомогателная модель для отображения данных.
	/// </summary>
	public class ApplicationViewModel : INotifyPropertyChanged
	{
		#region Fields

		/// <summary>
		/// Флаг инициализации.
		/// </summary>
		private bool IsInit = false;

		/// <summary>
		/// Флаг загрузки.
		/// </summary>
		private bool isBusy;

		/// <summary>
		/// Событие для оповещения об изменениях состояния.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Список парковок. 
		/// </summary>
		public ObservableCollection<Parking> ParkingPlaces
		{
			get => _parkingPlaces;
			set => _parkingPlaces = value;
		}

		/// <summary>
		/// Сервис для работы с API.
		/// </summary>
		private ParkingService _parkingService;

		/// <summary>
		/// Выбранная парковка.
		/// </summary>
		private Parking selectedParking;

		/// <summary>
		/// Выбранная парковка.
		/// </summary>
		public DetailParking SelectedDetailParking
		{
			get => _selectedDetailParking;
			private set => _selectedDetailParking = value;
		}

		/// <summary>
		/// Идентификатор выбранной парковки.
		/// </summary>
		private int selectedId;

		/// <summary>
		/// Для навигации между страницами.
		/// </summary>
		public INavigation Navigation
		{
			get => _navigation;
			set => _navigation = value;
		}

		/// <summary>
		/// Команда назад.
		/// </summary>
		public ICommand BackCommand
		{
			protected set => _backCommand = value;
			get => _backCommand;
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
					selectedId = value.Id;
					var tempParking = new Parking
					{
						Id = value.Id,
						Address = value.Address,
						FreeParkingSpaces = value.FreeParkingSpaces,
						TotalParkingSpaces = value.TotalParkingSpaces
					};
					//selectedParking = null;
					_selectedDetailParking = null;
					NotifyPropertyChanged("SelectedParking");
					// TODO: Передать сюда страницу с детальной парковкой
					Navigation.PushAsync(new ParkingPage(this));
				}
			}
		}

		/// <summary>
		/// Выбранная парковка.
		/// </summary>
		private DetailParking _selectedDetailParking;

		private ObservableCollection<Parking> _parkingPlaces;
		private INavigation _navigation;
		private ICommand _backCommand;

		/// <summary>
		/// Флаг загрузки.
		/// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				NotifyPropertyChanged("IsBusy");
				//NotifyPropertyChanged("IsLoaded");
			}
		}

		public bool IsLoaded => !isBusy;

		#endregion
		
		/// <summary>
		/// Конструктор.
		/// </summary>
		public ApplicationViewModel()
		{
			ParkingPlaces = new ObservableCollection<Parking>();
			_parkingService = new ParkingService();
			BackCommand = new Command(Back);
		}

		/// <summary>
		/// Получает все парковки.
		/// </summary>
		public async Task GetParkings()
		{
			if (IsInit)
				return;
			IsBusy = true;
			var parkings = await _parkingService.GetAll();
			foreach (var parking in parkings)
			{
				ParkingPlaces.Add(parking);
			}

			IsBusy = false;
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

		/// <summary>
		/// Получение парковки по Id.
		/// </summary>
		public async Task GetParkById()
		{
			IsBusy = true;
			SelectedDetailParking = await _parkingService.Get(selectedId);
			NotifyPropertyChanged("SelectedDetailParking");
			IsBusy = false;
		}
	}
}