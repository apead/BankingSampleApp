using BankingApp.Enums;
using BankingApp.Models;
using BankingApp.Services;

namespace BankingApp.Views;

public partial class TransactionRecordView : ContentPage, IQueryAttributable
{

	private BankingDatabaseService bankingDatabaseService;

	private BankAccount bankAccount;

	public BankAccount BankAccount
	{
		get { return bankAccount; }
		set { bankAccount = value;

			OnPropertyChanged();
		}
	}

	private TransactionTypeEnum transactionTypeEnum;

	public TransactionTypeEnum SelectedTransactionType
	{
		get { return transactionTypeEnum; }
		set { transactionTypeEnum = value; }
	}

	private string reference;

	public string Reference
	{
		get { return reference; }
		set { reference = value;
			OnPropertyChanged();

		}
	}

	private decimal amount;

	public decimal Amount
	{
		get { return amount; }
		set { amount = value;

			OnPropertyChanged();
		}
	}




	public TransactionRecordView(BankingDatabaseService service)
	{
		InitializeComponent();

		bankingDatabaseService = service;

        BindingContext = this;
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        BankAccount = query["BankAccount"] as BankAccount;

		SelectedTransactionType = (TransactionTypeEnum) query["TransactionType"];
    }

    private async void SubmitClicked(object sender, EventArgs e)
    {
		try
		{


			await bankingDatabaseService.MakeTransactionAsync(BankAccount.BankAccountId, reference, amount, transactionTypeEnum);

			await Shell.Current.GoToAsync("..");
		}
		catch(Exception ex)
		{

		}
    }
}