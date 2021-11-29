using System;
using ParkingApp.ViewModel;
using Xamarin.Forms;

namespace ParkingApp.Pages
{
	/// <summary>
	/// Главная страница приложения.
	/// </summary>
	public partial class MainPage : ContentPage
	{
		/// <summary>
		/// Вспомогательная модель.
		/// </summary>
		private ApplicationViewModel ViewModel { get; set; }

		/// <summary>
		/// Таймер для обновления.
		/// </summary>
		//private static System.Timers.Timer _updaterTimer;

		/// <summary>
		/// Конструктор.
		/// </summary>
		public MainPage()
		{
			InitializeComponent();
			ViewModel = new ApplicationViewModel {Navigation = this.Navigation};
			BindingContext = ViewModel;
		}

		/// <summary>
		/// Срабатывает при отрисовке страницы.
		/// </summary>
		protected override async void OnAppearing()
		{
			await ViewModel.GetParkingPlaces();
			ViewModel.Sorting();
			//SetTimer();
			//Устанавливаем соединение для обновлений
			await ViewModel.Connect();
			base.OnAppearing();
			ParkingSearch.Text = string.Empty;
		}

		protected override async void OnDisappearing()
		{
			//StopTimer();
			//Разрываем соединение
			await ViewModel.Disconnect();
			base.OnDisappearing();
		}

		/// <summary>
		/// Фильтрует коллекцию по адресу.
		/// </summary>
		private void UpdateSearch_Event(object sender, EventArgs e)
		{
			ViewModel.SelectByAddress(ParkingSearch.Text);
		}

		/// <summary>
		/// Событие при обновлении списка.
		/// </summary>
		private void ParkingList_OnRefreshing(object sender, EventArgs e)
		{
			ParkingSearch.Text = string.Empty;
		}

		/// <summary>
		/// Таймер для обновления.
		/// </summary>
		//private void SetTimer()
		//{
		//	_updaterTimer = new System.Timers.Timer(10000);
		//	_updaterTimer.Elapsed += UpdateParkingPlaces;
		//	_updaterTimer.AutoReset = true;
		//	_updaterTimer.Enabled = true;
		//}

		/// <summary>
		/// Останавливает таймер.
		/// </summary>
		//private void StopTimer()
		//{
		//	_updaterTimer.Stop();
		//}

		/// <summary>
		/// Обновляет список.
		/// </summary>
		private async void UpdateParkingPlaces(object sender, EventArgs e)
		{
			await ViewModel.UpdateList();
		}
	}
}