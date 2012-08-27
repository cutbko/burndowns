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
using Microsoft.Xna.Framework.Input.Touch;
using MoneyBurnDown.ViewModel;
using GestureEventArgs = Microsoft.Phone.Controls.GestureEventArgs;

namespace MoneyBurnDown.View
{
    public partial class ChartFullscreenView : PhoneApplicationPage
    {
        private bool _isScaleStarted;
        public ChartFullscreenView()
        {
            InitializeComponent();
            Touch.FrameReported += TouchFrameReported;
            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.VerticalDrag | GestureType.HorizontalDrag | GestureType.Pinch | GestureType.PinchComplete;

        }

        #region REWRITE THIS!
        void TouchFrameReported(object sender, TouchFrameEventArgs e)
        {

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gestureSample = TouchPanel.ReadGesture();
                if(gestureSample.GestureType == GestureType.Pinch)
                {
                    if(!_isScaleStarted)
                    {
                        ScaleTransform.CenterX = (gestureSample.Position.X + gestureSample.Position2.X) / 2;
                        ScaleTransform.CenterY = (gestureSample.Position.Y + gestureSample.Position2.Y) / 2;
                        _isScaleStarted = true;
                    }
                    else
                    {
                        ScaleTransform.ScaleX += -0.1;
                        ScaleTransform.ScaleY += -0.1;
                    }
                }
            }
        }
        #endregion


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("burndownId"))
            {
                int burndownId = int.Parse(NavigationContext.QueryString["burndownId"]);
                ViewModel.Initialize(burndownId);
            }
        }

        private ChartFullScreenViewModel ViewModel
        {
            get { return DataContext as ChartFullScreenViewModel; }
        }
    }
}