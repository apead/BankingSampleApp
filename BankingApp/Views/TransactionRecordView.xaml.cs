using BankingApp.Enums;
using BankingApp.Models;
using BankingApp.Services;
using System.Globalization;

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
		set { transactionTypeEnum = value;

			OnPropertyChanged();
			OnPropertyChanged(nameof(SubmitButtonText));

			if (transactionTypeEnum == TransactionTypeEnum.Deposit)
			{
				SubmitButton.Style = (Style)Resources["DepositButtonStyle"];
			}
			else
			{
				SubmitButton.Style = (Style)Resources["WithdrawalButtonStyle"];
			}
		}
	}

	public string SubmitButtonText
	{
		get
		{
			if (transactionTypeEnum == TransactionTypeEnum.Deposit)
			{
				return "Submit Deposit";
			}
			else
			{
				return "Submit Withdrawal";
			}
		}
	}

	private string reference;

	public string Reference
	{
		get { return reference; }
		set { reference = value;
			OnPropertyChanged();

		}
	}

	private string amount;

	public string Amount
	{
		get { return amount; }
		set { amount = value;

			OnPropertyChanged();
		}
	}

	private string referenceError;

	public string ReferenceError
	{
		get { return referenceError; }
		set { referenceError = value;

			OnPropertyChanged();
			OnPropertyChanged(nameof(HasReferenceError));
		}
	}

	public bool HasReferenceError
	{
		get { return !string.IsNullOrEmpty(referenceError); }
	}

	private string amountError;

	public string AmountError
	{
		get { return amountError; }
		set { amountError = value;

			OnPropertyChanged();
			OnPropertyChanged(nameof(HasAmountError));
		}
	}

	public bool HasAmountError
	{
		get { return !string.IsNullOrEmpty(amountError); }
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

	private void AmountTextChanged(object sender, TextChangedEventArgs e)
	{
		Entry entry = sender as Entry;

		string newText = e.NewTextValue;

		if (string.IsNullOrEmpty(newText))
		{
			return;
		}

		if (!IsValidAmountInput(newText))
		{
			entry.Text = e.OldTextValue;
		}
	}

	private bool IsValidAmountInput(string text)
	{
		int separatorCount = 0;
		int decimalDigits = 0;

		foreach (char character in text)
		{
			if (character == '.' || character == ',')
			{
				separatorCount = separatorCount + 1;

				if (separatorCount > 1)
				{
					return false;
				}
			}
			else if (char.IsDigit(character))
			{
				if (separatorCount == 1)
				{
					decimalDigits = decimalDigits + 1;

					if (decimalDigits > 2)
					{
						return false;
					}
				}
			}
			else
			{
				return false;
			}
		}

		return true;
	}

	private bool ValidateInput(out decimal parsedAmount)
	{
		bool isValid = true;
		parsedAmount = 0;

		ReferenceError = null;
		AmountError = null;

		if (string.IsNullOrWhiteSpace(Reference))
		{
			ReferenceError = "Please enter a reference.";
			isValid = false;
		}
		else if (Reference.Trim().Length > 50)
		{
			ReferenceError = "Reference cannot be longer than 50 characters.";
			isValid = false;
		}

		if (string.IsNullOrWhiteSpace(Amount))
		{
			AmountError = "Please enter an amount.";
			isValid = false;
		}
		else
		{
			string normalisedAmount = Amount.Replace(',', '.');

			if (!decimal.TryParse(normalisedAmount, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsedAmount))
			{
				AmountError = "Please enter a valid amount.";
				isValid = false;
			}
			else if (parsedAmount <= 0)
			{
				AmountError = "Amount must be greater than zero.";
				isValid = false;
			}
			else if (transactionTypeEnum == TransactionTypeEnum.Withdrawl && parsedAmount > BankAccount.Balance)
			{
				AmountError = "Insufficient funds. Amount exceeds the current balance.";
				isValid = false;
			}
		}

		return isValid;
	}

    private async void SubmitClicked(object sender, EventArgs e)
    {
		decimal parsedAmount;

		if (!ValidateInput(out parsedAmount))
		{
			return;
		}

		string transactionName;

		if (transactionTypeEnum == TransactionTypeEnum.Deposit)
		{
			transactionName = "deposit";
		}
		else
		{
			transactionName = "withdrawal";
		}

		string confirmationMessage = string.Format("Are you sure you want to make a {0} of {1:C2} on account {2}?\n\nReference: {3}", transactionName, parsedAmount, BankAccount.AccountNumber, Reference.Trim());

		bool confirmed = await DisplayAlertAsync("Confirm Transaction", confirmationMessage, "Yes", "Cancel");

		if (!confirmed)
		{
			return;
		}

		try
		{
			SubmitButton.IsEnabled = false;

			await bankingDatabaseService.MakeTransactionAsync(BankAccount.BankAccountId, Reference.Trim(), parsedAmount, transactionTypeEnum);

			await Shell.Current.GoToAsync("..");
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Transaction Failed", ex.Message, "OK");
		}
		finally
		{
			SubmitButton.IsEnabled = true;
		}
    }
}