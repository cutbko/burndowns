using System;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MoneyBurnDown.Model;
using MoneyBurnDown.Model.Entities;
using MoneyBurnDown.Model.Messages;

namespace MoneyBurnDown.ViewModel
{
    public class CreateTransactionViewModel : ValidatedViewModelBase
    {
        private readonly IMoneyDataSource _moneyDataSource;
        private Burndown _currentBurndown;
        private DateTime _transactionDate;
        private decimal _amount;
        private ICommand _createNewCommand;

        public CreateTransactionViewModel(IMoneyDataSource moneyDataSource)
        {
            _moneyDataSource = moneyDataSource;
            if(IsInDesignMode)
            {
                Initialize(1);
            }

            TransactionDate = DateTime.Now;
        }

        public void Initialize(int burndownId)
        {
            CurrentBurndown = _moneyDataSource.Burndowns.Single(x => x.Id == burndownId);
        }

        public Burndown CurrentBurndown
        {
            get { return _currentBurndown; }
            set
            {
                if(_currentBurndown != value)
                {
                    _currentBurndown = value;
                    RaisePropertyChanged(() => CurrentBurndown);
                }
            }
        }

        public string Header
        {
            get { return string.Format("The date must be between {0} and {1}", CurrentBurndown.StartDate.ToShortDateString(), CurrentBurndown.EndDate.ToShortDateString()); }
        }

        public DateTime TransactionDate
        {
            get { return _transactionDate; }
            set
            {
                if (_transactionDate != value)
                {
                    _transactionDate = value;
                    RaisePropertyChanged(() => TransactionDate);
                }
            }
        }

        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    RaisePropertyChanged(() => Amount);
                }
            }
        }

        public override string Validate()
        {
            if (TransactionDate < CurrentBurndown.StartDate || TransactionDate > CurrentBurndown.EndDate)
            {
                return "Select the date between end and start date";
            }

            if (Amount <= 0)
            {
                return "Amount must be more than zero";
            }

            return null;
        }

        public ICommand CreateNewCommand
        {
            get { return _createNewCommand ?? (_createNewCommand = new RelayCommand(CreateNew)); }
        }

        private void CreateNew()
        {
            CurrentBurndown.Transactions.Add(new Transaction
                                                 {
                                                     Amount = Amount,
                                                     CreatedAt = TransactionDate
                                                 });
            _moneyDataSource.SubmitChanges();
            Messenger.Default.Send(new TransactionCreatedMessage());
            Messenger.Default.Send(new RefreshBurndownsMessage());
        }
    }
}