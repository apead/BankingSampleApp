using BankingApp.Models;
using BankingApp.Services;
using System.Collections.ObjectModel;

namespace BankingApp.Views;

public partial class CustomerListView : ContentPage
{
	private BankingDatabaseService bankingDatabaseService;

    private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
    private int customerSearchVersion;

	public ObservableCollection<Customer> Customers
	{
		get { return customers; }

		set { customers = value;
			OnPropertyChanged();
		}
	}


	public CustomerListView(BankingDatabaseService service)
	{
		bankingDatabaseService = service;

		InitializeComponent();

        BindingContext = this;

    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await LoadCustomersAsync(CustomerSearchBar.Text);
    }

    private async void CustomerSearchBar_TextChanged(object? sender, TextChangedEventArgs e)
    {
        await LoadCustomersAsync(e.NewTextValue);
    }

    private async Task LoadCustomersAsync(string? searchText)
    {
        int searchVersion = ++customerSearchVersion;
        string name = searchText?.Trim() ?? string.Empty;

        if (name.Length > 0 && name.Length < 3)
        {
            return;
        }

        try
        {
            List<Customer> matchingCustomers =
                await bankingDatabaseService.SearchCustomersByLastNameAsync(name);

            if (searchVersion == customerSearchVersion)
            {
                Customers = new ObservableCollection<Customer>(matchingCustomers);
            }
        }
        catch (SQLite.SQLiteException exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            if (searchVersion == customerSearchVersion)
            {
                await DisplayAlertAsync("Unable to load customers", exception.Message, "OK");
            }
        }
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            CollectionView collectionView = sender as CollectionView;

            if (collectionView != null)
                collectionView.SelectedItem = null;


            Customer customer = e.CurrentSelection[0] as Customer;

            if (customer != null)
            {

                var navigationParameter = new ShellNavigationQueryParameters
    {
        { "Customer", customer }
    };

                await Shell.Current.GoToAsync("customeraccounts", navigationParameter);
            }
        }
    }
}