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
			await ViewModel.GetParkings();
			base.OnAppearing();
		}
	}
}