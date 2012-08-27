using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MoneyBurnDown.ViewModel;

namespace MoneyBurnDown.View
{
    public partial class ShowBurndownView : PhoneApplicationPage
    {
        public ShowBurndownView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("burndownId"))
            {
                int burndownId = int.Parse(NavigationContext.QueryString["burndownId"]);
                ViewModel.Initialize(burndownId);
                Messenger.Default.Register<Uri>(this, "NavigationRequest", uri => NavigationService.Navigate(uri));
                SetPinbutton(ViewModel.IsPinned);
                ViewModel.Pinned += ViewModelPinned;
            }
        }

        void ViewModelPinned(object sender, Infrastructure.PinEventArgs e)
        {
            bool isPinned = e.IsPinned;
            SetPinbutton(isPinned);
        }

        private void SetPinbutton(bool isPinned)
        {
            if (isPinned)
            {
                PinButton.Text = "Unpin";
                PinButton.IconUri = new Uri("/Images/appbar.unpin.png", UriKind.Relative);
            }
            else
            {
                PinButton.Text = "Pin";
                PinButton.IconUri = new Uri("/Images/appbar.pin.png", UriKind.Relative);
            }
        }

        private ShowBurndownViewModel ViewModel
        {
            get { return DataContext as ShowBurndownViewModel; }
        }
    }
}