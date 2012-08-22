using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    public partial class CreateBurndownView : PhoneApplicationPage
    {
        private readonly bool _isInitialized;
        public CreateBurndownView()
        {
            InitializeComponent();
            Messenger.Default.Register<RefreshBurndownsMessage>(this, message => NavigationService.GoBack());
            _isInitialized = true;
        }

        private ValidatedViewModelBase ViewModel
        {
            get { return DataContext as ValidatedViewModelBase; }
        }

        private void DateTimePickerBase_OnValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DatePicker box = (DatePicker)sender;
            BindingExpression be = box.GetBindingExpression(DatePicker.ValueProperty);
            be.UpdateSource();
            Validate();
        }

        private void Validate()
        {
            if (!_isInitialized)
                return;
            string validate = ViewModel.Validate();
            if (validate == null)
            {
                CreateButton.IsEnabled = true;
                ValidationError.Text = string.Empty;
            }
            else
            {
                CreateButton.IsEnabled = false;
                ValidationError.Text = validate;
            }
        }

        private void ListPickerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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