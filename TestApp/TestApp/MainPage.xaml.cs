using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestApp
{
	public partial class MainPage : ContentPage
	{
		public List<Parking> Parkings { get; set; }
		public ApplicationViewModel ViewModel { get; set; }
		public MainPage()
		{
			InitializeComponent();
			// ViewModel = new ApplicationViewModel();
			// Parkings = ViewModel.Parkings;
			ViewModel = new ApplicationViewModel();
			this.BindingContext = ViewModel;
		}
		
		
		// protected override async void OnAppearing()
		// {
		// 	//base.OnAppearing();
		// 	Parkings = new List<Parking>
		// 	{
		// 		new Parking {Address = "Ленина", Id = 1, TotalParkingSpaces = 10, FreeParkingSpaces = 5 },
		// 		new Parking {Address = "Ленина", Id = 1, TotalParkingSpaces = 10, FreeParkingSpaces = 5 },
		// 		new Parking {Address = "Ленина", Id = 1, TotalParkingSpaces = 10, FreeParkingSpaces = 5 },
		// 		new Parking {Address = "Ленина", Id = 1, TotalParkingSpaces = 10, FreeParkingSpaces = 5 }
		// 	};
		// }
	}
}