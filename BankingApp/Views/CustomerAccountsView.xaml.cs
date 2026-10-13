using BankingApp.Models;
using BankingApp.Services;
using System.Collections.ObjectModel;

namespace BankingApp.Views;

public partial class CustomerAccountsView : ContentPage, IQueryAttributable
{
	private BankingDatabaseService bankingDatabaseService;

	private Customer? customer;

	public Customer? Customer
	{
		get { return customer; }
		set {
			customer = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(CustomerFullName));
		}
	}

	public string CustomerFullName
	{
		get
		{
			if (Customer != null)
			{
				return $"{Customer.FirstName} {Customer.LastName}".Trim();
			}
			else
			{
				return string.Empty;
			}
		}
	}

	private ObservableCollection<BankAccount> customerAccounts = new ObservableCollection<BankAccount>();

	public ObservableCollection<BankAccount> CustomerAccounts
	{
		get { return customerAccounts; }
		set { customerAccounts = value;
			OnPropertyChanged();
		}
	}

	public CustomerAccountsView(BankingDatabaseService service)
	{
		InitializeComponent();

		bankingDatabaseService = service;

		BindingContext = this;
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		Customer = query["Customer"] as Customer;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		await LoadAccountsAsync();
	}

	private async Task LoadAccountsAsync()
	{
		if (customer != null)
		{
			var accounts = await bankingDatabaseService.GetAccountsByCustomerAsync(customer.CustomerId);
			CustomerAccounts = new ObservableCollection<BankAccount>(accounts);
		}
	}

	private async void CollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.Count > 0)
		{

			CollectionView collectionView = sender as CollectionView;

     		if (collectionView != null)
                collectionView.SelectedItem = null;

            BankAccount? bankAccount = e.CurrentSelection[0] as BankAccount;

			if (bankAccount != null)
			{
				var navigationParameter = new ShellNavigationQueryParameters
				{
					{ "BankAccount", bankAccount }
				};

				if (Customer != null)
				{
					navigationParameter.Add("Customer", Customer);
				}

				await Shell.Current.GoToAsync("bankaccounttransactions", navigationParameter);
			}

		
		}
	}
}