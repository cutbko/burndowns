using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MoneyBurnDown.Model;
using MoneyBurnDown.Model.Messages;

namespace MoneyBurnDown.ViewModel
{
    public class CreateTransactionTypeViewModel : ValidatedViewModelBase
    {
         private readonly IMoneyDataSource _dataSource;
        private string _name;
        private ICommand _createCommand;

        public CreateTransactionTypeViewModel(IMoneyDataSource dataSource)
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
            _dataSource.AddTransactionType(Name);
            _dataSource.SubmitChanges();
            Messenger.Default.Send(new TransactionTypeCreatedMessage());
        }

        public override string Validate()
        {
            if(string.IsNullOrWhiteSpace(Name))
            {
                return "Name cannot be null or empty";
            }

            if(_dataSource.TransactionTypes.Any(x=>x.Name == Name))
            {
                return "Such type already exists";
            }

            return null;
        }
    }
}