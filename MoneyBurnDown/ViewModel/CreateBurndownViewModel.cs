using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MoneyBurnDown.Model;
using MoneyBurnDown.Model.Entities;
using MoneyBurnDown.Model.Messages;

namespace MoneyBurnDown.ViewModel
{
    public class CreateBurndownViewModel : ValidatedViewModelBase
    {
        private readonly IMoneyDataSource _dataSource;
        private DateTime _startDate;
        private DateTime _endDate;
        private BurndownType _burndownType;
        private decimal _moneyToSpend;
        private ICommand _createCommand;
        private string _name;

        public CreateBurndownViewModel(IMoneyDataSource dataSource)
        {
            _dataSource = dataSource;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    RaisePropertyChanged(() => StartDate);
                }
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    RaisePropertyChanged(() => EndDate);
                }
            }
        }

        public BurndownType BurndownType
        {
            get { return _burndownType; }
            set
            {
                if (_burndownType != value)
                {
                    _burndownType = value;
                    RaisePropertyChanged(() => BurndownType);
                }
            }
        }

        public IEnumerable<BurndownType> BurndownTypes
        {
            get { return _dataSource.BurndownTypes.ToList(); }
        }

        public decimal MoneyToSpend
        {
            get { return _moneyToSpend; }
            set
            {
                if(_moneyToSpend != value)
                {
                    _moneyToSpend = value;
                    RaisePropertyChanged(()=>MoneyToSpend);
                }
            }
        }

        public override string Validate()
        {
            if(string.IsNullOrWhiteSpace(Name))
            {
                return "Please enter burndown name";
            }

            if(EndDate.Date <= StartDate.Date)
            {
                return "Due date must be more than start date";
            }

            if(MoneyToSpend<=0)
            {
                return "You should have money to spend";
            }

            return null;
        }

        public ICommand CreateCommand
        {
            get { return _createCommand ?? (_createCommand = new RelayCommand(Create)); }
        }

        private void Create()
        {
            _dataSource.CreateBurndown(Name, StartDate, EndDate, MoneyToSpend, BurndownType);
            Messenger.Default.Send(new RefreshBurndownsMessage());
        }
    }
}