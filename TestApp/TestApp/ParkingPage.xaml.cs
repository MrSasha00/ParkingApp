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
		public Parking Model { get; set; }
		public DetailParking DetModel { get; set; }
		public ApplicationViewModel ViewModel { get; set; }
		public ParkingPage(Parking model, ApplicationViewModel viewModel)
		{
			InitializeComponent();
			Model = model;
			DetModel = viewModel.selectedDetailParking;
			ViewModel = viewModel;
			this.BindingContext = this;
		}

		protected override async void OnAppearing()
		{
			// TODO: тут заполнять текущую парковку
			await ViewModel.GetParkById();
			base.OnAppearing();
		}
	}
}