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
			base.OnAppearing();
			ParkingSearch.Text = string.Empty;
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
	}
}