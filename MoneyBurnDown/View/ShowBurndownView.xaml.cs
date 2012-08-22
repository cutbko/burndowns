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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using MoneyBurnDown.ViewModel;

namespace MoneyBurnDown.View
{
    public partial class ShowBurndownView : PhoneApplicationPage
    {
        private readonly GestureListener _gestureListener;
        public ShowBurndownView()
        {
            InitializeComponent();
            _gestureListener = GestureService.GetGestureListener(this);
            _gestureListener.PinchStarted += ShowBurndownView_PinchStarted;
            _gestureListener.PinchDelta += _gestureListener_PinchDelta;
            _gestureListener.PinchCompleted += _gestureListener_PinchCompleted;
        }

        void _gestureListener_PinchCompleted(object sender, PinchGestureEventArgs e)
        {
        }

        void _gestureListener_PinchDelta(object sender, PinchGestureEventArgs e)
        {
        }

        void ShowBurndownView_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("burndownId"))
            {
                int burndownId = int.Parse(NavigationContext.QueryString["burndownId"]);
                ViewModel.Initialize(burndownId);
                Messenger.Default.Register<Uri>(this, "NavigationRequest", uri => NavigationService.Navigate(uri));
            }
        }

        private ShowBurndownViewModel ViewModel
        {
            get { return DataContext as ShowBurndownViewModel; }
        }

        private void chartControl_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
    }
}