using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Shell;
using MoneyBurnDown.Controls;
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

        private Uri _navigationUri;
        private ICommand _showChartLegendCommand;

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
                                                                                RaisePropertyChanged(() => MoneyLeft);
                                                                                RaisePropertyChanged(() => DaysLeft);
                                                                                RaisePropertyChanged(() => MoneyPerDayLeft);
                                                                                if(IsPinned)
                                                                                {
                                                                                    ShellTile.ActiveTiles.Single(x=>x.NavigationUri == _navigationUri).Update(TileData);
                                                                                }
                                                                            });
        }

        public void Initialize(int burndownId)
        {
            CurrentBurndown = _moneyDataSource.Burndowns.Single(x => x.Id == burndownId);
            _navigationUri = new Uri(string.Format("/View/ShowBurndownView.xaml?burndownId={0}", burndownId), UriKind.Relative);
        }

        public decimal DaysLeft
        {
            get { return CurrentBurndown.DaysLeft; }
        }

        public decimal MoneyLeft
        {
            get { return CurrentBurndown.MoneyLeft; }
        }

        public decimal MoneyPerDayLeft
        {
            get { return CurrentBurndown.MoneyPerDayLeft; }
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
                    RaisePropertyChanged(() => MoneyLeft);
                    RaisePropertyChanged(() => DaysLeft);
                    RaisePropertyChanged(() => MoneyPerDayLeft);
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
            ShellTile shellTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri == _navigationUri);
            
            if(shellTile != null)
            {
                shellTile.Delete();
                OnPinned(false);
            }
            else
            {
                ShellTile.Create(_navigationUri, TileData);
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
                               BackContent = CurrentBurndown.MoneyLeft.ToString(CultureInfo.InvariantCulture),
                               BackgroundImage = CreateLiveTileImage()
                           };
            }
        }

        private Uri CreateLiveTileImage()
        {
            ChartCanvas chartCanvas = new ChartCanvas
                                          {
                                              Burndown = CurrentBurndown, 
                                              Width = 173, 
                                              Height = 173
                                          };

            chartCanvas.CreateGraph(new Rect(0, 0, 173, 173));

            WriteableBitmap bmp = new WriteableBitmap(173, 173);
            bmp.Render(chartCanvas, null);
            bmp.Invalidate();

            var isf = IsolatedStorageFile.GetUserStoreForApplication();
            var filename = "/Shared/ShellContent/tile" + CurrentBurndown.Id + ".jpg";
            if (!isf.DirectoryExists("/Shared/ShellContent"))
            {
                isf.CreateDirectory("/Shared/ShellContent");
            }
            using (var stream = isf.OpenFile(filename, System.IO.FileMode.OpenOrCreate))
            {
                bmp.SaveJpeg(stream, 173, 173, 0, 100);
            }

            return new Uri("isostore:" + filename, UriKind.Absolute);
        }

        public bool IsPinned
        {
            get { return ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri == _navigationUri) != null; }
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
                }
            }
        }

        public event EventHandler<PinEventArgs> Pinned;

        protected virtual void OnPinned(bool isPinned)
        {
            EventHandler<PinEventArgs> handler = Pinned;
            if (handler != null) handler(this, new PinEventArgs {IsPinned = isPinned});
        }

        public ICommand ShowChartLegendCommand
        {
            get { return _showChartLegendCommand ?? (_showChartLegendCommand = new RelayCommand(ShowChartLegend)); }
        }

        private void ShowChartLegend()
        {
            Messenger.Default.Send(new Uri("/View/ShowChartLegendView.xaml", UriKind.Relative), "NavigationRequest");
        }
    }
}