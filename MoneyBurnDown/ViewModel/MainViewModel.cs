using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MoneyBurnDown.Model;
using MoneyBurnDown.Model.Entities;
using MoneyBurnDown.Model.Messages;

namespace MoneyBurnDown.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IMoneyDataSource _dataSource;
        private int _selectedView;
        private ICommand _createNewCommand;
        private readonly Burndown _selectedBurndown = null;
        private ICommand _deleteTransactionType;
        private readonly TransactionType _selectedTransactionType = null;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IMoneyDataSource dataSource)
        {
            _dataSource = dataSource;
            Messenger.Default.Register<TypeCreatedMessage>(this, message => RaisePropertyChanged(() => BurndownTypes));
            Messenger.Default.Register<TransactionTypeCreatedMessage>(this, message => RaisePropertyChanged(() => TransactionTypes));
            Messenger.Default.Register<RefreshBurndownsMessage>(this, message => RaisePropertyChanged(() => Burndowns));
        }

        public IEnumerable<Burndown> Burndowns
        {
            get { return _dataSource.Burndowns.ToList(); }
        }

        public IEnumerable<BurndownType> BurndownTypes
        {
            get { return _dataSource.BurndownTypes.ToList(); }
        }

        public IEnumerable<TransactionType> TransactionTypes
        {
            get { return _dataSource.TransactionTypes.ToList(); }
        } 

        public int SelectedView
        {
            get { return _selectedView; }
            set
            {
                if (_selectedView != value)
                {
                    _selectedView = value;
                    RaisePropertyChanged(() => SelectedView);
                }
            }
        }

        public Burndown SelectedBurndown
        {
            get { return _selectedBurndown; }
            set
            {
                RaisePropertyChanged(() => SelectedBurndown);
                if (value != null)
                {
                    Messenger.Default.Send(new Uri("/View/ShowBurndownView.xaml?burndownId=" + value.Id, UriKind.Relative), "NavigationRequest");
                }
            }
        }

        public TransactionType SelectedTransactionType
        {
            get { return _selectedTransactionType; }
            set
            {
                RaisePropertyChanged(()=>SelectedTransactionType);
                if(value != null)
                {
                    
                }
            }
        }

        public ICommand CreateNewCommand
        {
            get { return _createNewCommand ?? (_createNewCommand = new RelayCommand(CreateNew)); }
        }

        private void CreateNew()
        {
            Messenger.Default.Send(GetUriForNew(), "NavigationRequest");
        }

        private Uri GetUriForNew()
        {
            switch (SelectedView)
            {
                case 0:
                    return new Uri("/View/CreateBurndownView.xaml", UriKind.Relative);
                case 1:
                    return new Uri("/View/CreateTypeView.xaml", UriKind.Relative);
                case 2:
                    return new Uri("/View/CreateTransactionTypeView.xaml", UriKind.Relative);
            }

            return null;
        }

        public ICommand DeleteTransactionTypeCommand
        {
            get
            {
                return _deleteTransactionType ?? (_deleteTransactionType = new RelayCommand<TransactionType>(DeleteTransactionType));
            }
        }

        private void DeleteTransactionType(TransactionType obj)
        {
            _dataSource.Transactions.Where(x=> x.TransactionType != null && x.TransactionType.Id == obj.Id).ToList().ForEach(x=>x.TransactionType = null);
            _dataSource.DeleteTransactionType(obj);
            RaisePropertyChanged(() => TransactionTypes);
        }
    }
}