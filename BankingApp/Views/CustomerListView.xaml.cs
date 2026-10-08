using BankingApp.Models;
using BankingApp.Services;
using System.Collections.ObjectModel;

namespace BankingApp.Views;

public partial class CustomerListView : ContentPage
{
	private BankingDatabaseService bankingDatabaseService;

	private ObservableCollection<Customer> customers = new();

	public ObservableCollection<Customer> Customers
	{
		get { return customers; }

		set { customers = value;
			OnPropertyChanged();
		}
	}


	public CustomerListView(BankingDatabaseService service)
	{
		InitializeComponent();

		bankingDatabaseService = service;

        BindingContext = this;

    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        Customers = new ObservableCollection<Customer>( 
			await bankingDatabaseService.GetAllCustomersAsync()
			);
    }


}