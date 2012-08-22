using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace MoneyBurnDown.View
{
    public partial class MainPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) => NavigationService.Navigate(uri));
        }
    }
}
