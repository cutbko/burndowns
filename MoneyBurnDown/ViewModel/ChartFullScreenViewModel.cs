using System.Linq;
using GalaSoft.MvvmLight;
using MoneyBurnDown.Model;
using MoneyBurnDown.Model.Entities;

namespace MoneyBurnDown.ViewModel
{
    public class ChartFullScreenViewModel : ViewModelBase
    {
        private readonly IMoneyDataSource _moneyDataSource;
        private Burndown _currentBurndown;

        public ChartFullScreenViewModel(IMoneyDataSource moneyDataSource)
        {
            _moneyDataSource = moneyDataSource;
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
                    RaisePropertyChanged(()=>CurrentBurndown);
                }
            }
        }
    }
}