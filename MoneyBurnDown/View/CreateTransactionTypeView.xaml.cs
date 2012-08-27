using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using MoneyBurnDown.Model.Messages;
using MoneyBurnDown.ViewModel;

namespace MoneyBurnDown.View
{
    public partial class CreateTransactionTypeView : PhoneApplicationPage
    {
        public CreateTransactionTypeView()
        {
            InitializeComponent();
            Messenger.Default.Register<TransactionTypeCreatedMessage>(this, message => NavigationService.GoBack());
        }

        private ValidatedViewModelBase ViewModel
        {
            get { return DataContext as ValidatedViewModelBase; }
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateSource();
            }

            Validate();
        }

        private void Validate()
        {
            ErrorsTB.Text = ViewModel.Validate();
            CreateButton.IsEnabled = string.IsNullOrWhiteSpace(ErrorsTB.Text);
        }
    }
}