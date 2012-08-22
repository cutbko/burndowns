using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using MoneyBurnDown.Model.Messages;

namespace MoneyBurnDown.View
{
    /// <summary>
    /// Description for CreateTypeView.
    /// </summary>
    public partial class CreateTypeView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the CreateTypeView class.
        /// </summary>
        public CreateTypeView()
        {
            InitializeComponent();
            Messenger.Default.Register<TypeCreatedMessage>(this, message => NavigationService.GoBack());
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            CreateButton.IsEnabled = ((TextBox) sender).Text.Length > 0;
        }
    }
}