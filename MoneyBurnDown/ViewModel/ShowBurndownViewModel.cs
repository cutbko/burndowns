using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Shell;
using MoneyBurnDown.Infrastructure;
using MoneyBurnDown.Model;
using MoneyBurnDown.Model.Entities;
using MoneyBurnDown.Model.Messages;

namespace MoneyBurnDown.ViewModel
{
    public class ShowBurndownViewModel : ViewModelBase
    {
        private readonly IMoneyDataSource _moneyDataSource;

        private Burndown _currentBurndown;
        private ICommand _createNewCommand;
        private ICommand _toogleFullscreenCommand;
        private int _selectedPivot;
        private ICommand _pinCommand;

        private Uri NavigationUri;

        public ShowBurndownViewModel(IMoneyDataSource moneyDataSource)
        {
            _moneyDataSource = moneyDataSource;
            if (IsInDesignMode)
            {
                Initialize(1);
            }
            Messenger.Default.Register<TransactionCreatedMessage>(this, message =>
                                                                            {
                                                                                RaisePropertyChanged(() => CurrentBurndown.Transactions);
                                                                                RaisePropertyChanged(() => DailyExpenses);
                                                                                RaisePropertyChanged(() => AverageDailyExpences);
                                                                                RaisePropertyChanged(() => ExpencesByType);
                                                                                RaisePropertyChanged(() => CurrentBurndown.MoneyLeft);
                                                                                if(IsPinned)
                                                                                {
                                                                                    ShellTile.ActiveTiles.Single(x=>x.NavigationUri == NavigationUri).Update(TileData);
                                                                                }
                                                                            });
        }

        public void Initialize(int burndownId)
        {
            CurrentBurndown = _moneyDataSource.Burndowns.Single(x => x.Id == burndownId);
            NavigationUri = new Uri(string.Format("/View/ShowBurndownView.xaml?burndownId={0}", burndownId), UriKind.Relative);
        }

        public Burndown CurrentBurndown
        {
            get { return _currentBurndown; }
            set
            {
                if (_currentBurndown != value)
                {
                    _currentBurndown = value;
                    RaisePropertyChanged(() => CurrentBurndown);
                    RaisePropertyChanged(() => CurrentBurndown.Name);
                    RaisePropertyChanged(() => CurrentBurndown.Transactions);
                    RaisePropertyChanged(() => DailyExpenses);
                    RaisePropertyChanged(() => AverageDailyExpences);
                    RaisePropertyChanged(() => ExpencesByType);
                }
            }
        }

        public string Name
        {
            get { return _currentBurndown.Name; }
        }

        public decimal AverageDailyExpences
        {
            get
            {
                if (CurrentBurndown != null)
                {
                    return DailyExpenses.Where(x => x.Key <= DateTime.Today).Average(x => x.Value);
                }

                return 0;
            }
        }

        public decimal ExpectedDailyExpences
        {
            get
            {
                if (CurrentBurndown != null)
                {
                    return CurrentBurndown.MoneyOnStart / (decimal) (CurrentBurndown.EndDate.Date - CurrentBurndown.StartDate.Date).TotalDays;
                }

                return 0;
            }
        }

        public IEnumerable<Transaction> Transactions
        {
            get { return _currentBurndown.Transactions.ToList(); }
        }

        public IEnumerable<KeyValuePair<DateTime, decimal>> DailyExpenses
        {
            get
            {
                if (CurrentBurndown != null)
                {
                    Dictionary<DateTime, decimal> expenses = CurrentBurndown.Transactions.GroupBy(x => x.CreatedAt.Date).Select(x => new { x.Key.Date, Amount = x.Sum(y => y.Amount) }).ToDictionary(x => x.Date, x => x.Amount);
                    for (DateTime date = CurrentBurndown.StartDate.Date; date <= CurrentBurndown.EndDate.Date; date = date.AddDays(1))
                    {
                        if (expenses.ContainsKey(date))
                        {
                            yield return new KeyValuePair<DateTime, decimal>(date, expenses[date]);
                        }
                        else
                        {
                            yield return new KeyValuePair<DateTime, decimal>(date, 0);
                        }
                    }
                }
            }
        }

        public IEnumerable<KeyValuePair<string, decimal>> ExpencesByType
        {
            get
            {
                if(CurrentBurndown != null)
                {
                    foreach (var source in CurrentBurndown.Transactions.GroupBy(x=>x.TransactionType).ToList())
                    {
                        if(source.Key == null)
                        {
                            yield return new KeyValuePair<string, decimal>("Not specified", source.Sum(x=>x.Amount));
                        }
                        else
                        {
                            yield return new KeyValuePair<string, decimal>(source.Key.Name, source.Sum(x=>x.Amount));
                        }
                    }
                }
            }
        }

        public ICommand CreateNewCommand
        {
            get { return _createNewCommand ?? (_createNewCommand = new RelayCommand(CreateNew)); }
        }

        private void CreateNew()
        {
            Messenger.Default.Send(new Uri("/View/CreateTransactionView.xaml?burndownId=" + CurrentBurndown.Id, UriKind.Relative), "NavigationRequest");
        }

        public ICommand PinCommand
        {
            get { return _pinCommand  ?? (_pinCommand = new RelayCommand(Pin)); }
        }

        private void Pin()
        {
            ShellTile shellTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri == NavigationUri);
            if(shellTile != null)
            {
                shellTile.Delete();
                OnPinned(false);
            }
            else
            {
                ShellTile.Create(NavigationUri, TileData);
                OnPinned(true);
            }
        }

        private StandardTileData TileData
        {
            get
            {
                return new StandardTileData
                           {
                               BackTitle = CurrentBurndown.EndDate.ToShortDateString(),
                               Title = CurrentBurndown.Name,
                               BackContent = CurrentBurndown.MoneyLeft.ToString(CultureInfo.InvariantCulture)
                           };
            }
        }

        public bool IsPinned
        {
            get { return ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri == NavigationUri) != null; }
        }

        public ICommand ToogleFullscreenCommand
        {
            get { return _toogleFullscreenCommand ?? (_toogleFullscreenCommand = new RelayCommand(ToogleFullscreen)); }
        }

        private void ToogleFullscreen()
        {
            Messenger.Default.Send(new Uri("/View/ChartFullscreenView.xaml?burndownId=" + CurrentBurndown.Id, UriKind.Relative), "NavigationRequest");
        }

        public int SelectedPivot
        {
            get { return _selectedPivot; }
            set
            {
                if(_selectedPivot != value)
                {
                    _selectedPivot = value;
                    RaisePropertyChanged(() => SelectedPivot);
                    RaisePropertyChanged(() => IsFullScreenVisible);
                }
            }
        }

        public bool IsFullScreenVisible
        {
            get { return SelectedPivot == 3; }
        }

        public event EventHandler<PinEventArgs> Pinned;

        protected virtual void OnPinned(bool isPinned)
        {
            EventHandler<PinEventArgs> handler = Pinned;
            if (handler != null) handler(this, new PinEventArgs {IsPinned = isPinned});
        }
    }
}