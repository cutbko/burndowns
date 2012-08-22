using System;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MoneyBurnDown.Model;
using MoneyBurnDown.Model.Entities;
using MoneyBurnDown.Model.Messages;

namespace MoneyBurnDown.ViewModel
{
    public class CreateTypeViewModel : ViewModelBase
    {
        private readonly IMoneyDataSource _dataSource;
        private string _name;
        private ICommand _createCommand;

        public CreateTypeViewModel(IMoneyDataSource dataSource)
        {
            _dataSource = dataSource;
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

        public ICommand CreateCommand
        {
            get { return _createCommand ?? (_createCommand = new RelayCommand(Create)); }
        }

        private void Create()
        {
            _dataSource.AddBurndownType(Name);
            _dataSource.SubmitChanges();
            Messenger.Default.Send(new TypeCreatedMessage {Name = Name});
        }
    }
}