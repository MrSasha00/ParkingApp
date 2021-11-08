using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ParkingPage : ContentPage
	{
		public ApplicationViewModel ViewModel { get; set; }
		public ParkingPage(ApplicationViewModel viewModel)
		{
			InitializeComponent();
			ViewModel = viewModel;
			this.BindingContext = this;
		}

		protected override async void OnAppearing()
		{
			await ViewModel.GetParkById();
			base.OnAppearing();
		}
	}
}