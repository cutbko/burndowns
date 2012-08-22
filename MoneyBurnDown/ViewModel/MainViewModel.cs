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
        private Burndown _selectedBurndown;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IMoneyDataSource dataSource)
        {
            _dataSource = dataSource;
            Messenger.Default.Register<TypeCreatedMessage>(this, message => RaisePropertyChanged(() => BurndownTypes));
            Messenger.Default.Register<RefreshBurndownsMessage>(this, message => RaisePropertyChanged(() => Burndowns));
        }

        //public override void Cleanup()
        //{
        //    // Clean up if needed

        //    base.Cleanup();
        //}

        public IEnumerable<Burndown> Burndowns
        {
            get { return _dataSource.Burndowns.ToList(); }
        }

        public IEnumerable<BurndownType> BurndownTypes
        {
            get { return _dataSource.BurndownTypes.ToList(); }
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

        public ICommand CreateNewCommand
        {
            get { return _createNewCommand ?? (_createNewCommand = new RelayCommand(CreateNew)); }
        }

        private void CreateNew()
        {
            Messenger.Default.Send(new Uri(SelectedView == 0 ? "/View/CreateBurndownView.xaml" : "/View/CreateTypeView.xaml", UriKind.Relative), "NavigationRequest");
        }
    }
}