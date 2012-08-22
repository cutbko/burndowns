using System;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using MoneyBurnDown.Model.Messages;
using MoneyBurnDown.ViewModel;

namespace MoneyBurnDown.View
{
    public partial class CreateTransactionView
    {
        private readonly bool _isInitialized;

        public CreateTransactionView()
        {
            InitializeComponent();
            _isInitialized = true;
            Messenger.Default.Register<TransactionCreatedMessage>(this, message => NavigationService.GoBack());
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("burndownId"))
            {
                int burndownId = int.Parse(NavigationContext.QueryString["burndownId"]);
                ViewModel.Initialize(burndownId);
                Messenger.Default.Register<Uri>(this, "NavigationRequest", uri => NavigationService.Navigate(uri));
            }
        }

        private CreateTransactionViewModel ViewModel
        {
            get { return DataContext as CreateTransactionViewModel; }
        }

        private void Validate()
        {
            if(!_isInitialized)
                return;

            string validationMessage = ViewModel.Validate();
            if (validationMessage == null)
            {
                CreateButton.IsEnabled = true;
                Errors.Text = string.Empty;
            }
            else
            {
                CreateButton.IsEnabled = false;
                Errors.Text = validationMessage;
            }
        }

        private void TranDateValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DatePicker box = (DatePicker)sender;
            BindingExpression be = box.GetBindingExpression(DatePicker.ValueProperty);
            be.UpdateSource();
            Validate();
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            BindingExpression be = box.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
            Validate();
        }
    }
}