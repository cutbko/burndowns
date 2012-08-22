using GalaSoft.MvvmLight;

namespace MoneyBurnDown.ViewModel
{
    public abstract class ValidatedViewModelBase : ViewModelBase
    {
        public abstract string Validate();
    }
}