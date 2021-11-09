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
			BindingContext = this;
		}

		/// <summary>
		/// Срабатывает при отрисовке страницы.
		/// </summary>
		protected override async void OnAppearing()
		{
			await ViewModel.GetParkById();
			base.OnAppearing();
		}
	}
}