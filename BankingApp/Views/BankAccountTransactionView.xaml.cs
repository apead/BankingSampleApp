using BankingApp.Enums;
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
		if (query.ContainsKey("BankAccount"))
		{
			BankAccount = query["BankAccount"] as BankAccount;
		}

		if (query.ContainsKey("Customer"))
		{
			Customer = query["Customer"] as Customer;
		}
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
			var bankingTransactions = await bankingDatabaseService.GetTransactionsByAccountAsync(BankAccount.BankAccountId);
			Transactions = new ObservableCollection<Transaction>(bankingTransactions);
		}
	}

    private async void DepositButtonClicked(object sender, EventArgs e)
    {
        var navigationParameter = new ShellNavigationQueryParameters
                {
                    { "BankAccount", bankAccount },
			        {"TransactionType", TransactionTypeEnum.Deposit }
                };

        await Shell.Current.GoToAsync("transactionrecord", navigationParameter);
    }

    private void WithdrawlButtonClicked(object sender, EventArgs e)
    {

    }
}