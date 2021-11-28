using System;
using System.IO;
using ParkingApp.ViewModel;
using Xamarin.Forms;

namespace ParkingApp.Pages
{
	/// <summary>
	/// Страница детаьной парковки.
	/// </summary>
	public partial class ParkingPage : ContentPage
	{
		/// <summary>
		/// Вспомогательная модель.
		/// </summary>
		private ApplicationViewModel ViewModel { get; set; }

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="viewModel">Вспомогательная модель для отображения.</param>
		public ParkingPage(ApplicationViewModel viewModel)
		{
			InitializeComponent();
			ViewModel = viewModel;
			BindingContext = ViewModel;
		}

		/// <summary>
		/// Срабатывает при отрисовке страницы.
		/// </summary>
		protected override async void OnAppearing()
		{
			await ViewModel.GetParkById();
			await ViewModel.UpdatePhoto();
			Image.Source = ImageSource.FromStream(() => new MemoryStream(ViewModel.Photo));
			ViewModel.Notify += UpdatePhoto;
			base.OnAppearing();
		}

		/// <summary>
		/// Срабатывает при уничтожении страницы.
		/// </summary>
		protected override void OnDisappearing()
		{
			ViewModel.Notify -= UpdatePhoto;
			base.OnDisappearing();
		}

		/// <summary>
		/// Обновление страницы.
		/// </summary>
		private async void Refresh(object sender, EventArgs e)
		{
			await ViewModel.GetParkById();
			RefreshView.IsRefreshing = false;
		}

		/// <summary>
		/// Обновляет фото в форме.
		/// </summary>
		private async void UpdatePhoto(int spaces)
		{
			await ViewModel.UpdatePhoto();
			await ViewModel.GetParkById();
			Device.BeginInvokeOnMainThread(() =>
				{
					Image.Source = ImageSource.FromStream(() => new MemoryStream(ViewModel.Photo));
					FreePlacesSpan.Text = spaces.ToString();
				}
			);
		}
	}
}