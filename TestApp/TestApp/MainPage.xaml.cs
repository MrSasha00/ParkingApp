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
		private ApplicationViewModel ViewModel { get; set; }
		public MainPage()
		{
			InitializeComponent();
			ViewModel = new ApplicationViewModel {Navigation = this.Navigation};
			this.BindingContext = ViewModel;
		}

		protected override async void OnAppearing()
		{
			await ViewModel.GetParkings();
			base.OnAppearing();
		}
	}
}