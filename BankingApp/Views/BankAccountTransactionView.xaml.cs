using BankingApp.Models;
using BankingApp.Services;
using System.Collections.ObjectModel;

namespace BankingApp.Views;

public partial class BankAccountTransactionView : ContentPage, IQueryAttributable
{
	private BankingDatabaseService bankingDatabaseService;

	private BankAccount? bankAccount;
	public BankAccount? BankAccount
	{
		get
		{
			return bankAccount;
		}
		set
		{
			bankAccount = value;
			OnPropertyChanged();
		}
	}

	private Customer? customer;
	public Customer? Customer
	{
		get
		{
			return customer;
		}
		set
		{
			customer = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(AccountHolderName));
		}
	}

	public string AccountHolderName
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

	private ObservableCollection<Transaction> transactions = new ObservableCollection<Transaction>();
	public ObservableCollection<Transaction> Transactions
	{
		get
		{
			return transactions;
		}
		set
		{
			transactions = value;
			OnPropertyChanged();
		}
	}

	public BankAccountTransactionView(BankingDatabaseService service)
	{
		InitializeComponent();

		bankingDatabaseService = service;

		BindingContext = this;
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		BankAccount = query["BankAccount"] as BankAccount;
		Customer = query["Customer"] as Customer;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		await LoadTransactionsAsync();
	}

	private async Task LoadTransactionsAsync()
	{
		if (BankAccount != null)
		{
			var txs = await bankingDatabaseService.GetTransactionsByAccountAsync(BankAccount.BankAccountId);
			Transactions = new ObservableCollection<Transaction>(txs);
		}
	}
}