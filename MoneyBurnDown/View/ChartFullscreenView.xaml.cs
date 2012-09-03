using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MoneyBurnDown.ViewModel;

namespace MoneyBurnDown.View
{
    public partial class ChartFullscreenView : PhoneApplicationPage
    {
        private bool _isScaleStarted;
        private bool _isDrugStarted;

        private float _gestWidth;
        private float _gestHeight;

        private CompositeTransform _compositeTransform;

        private Vector2 _drugPosition;

        public ChartFullscreenView()
        {
            InitializeComponent();
            //Touch.FrameReported += TouchFrameReported;
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
                        _compositeTransform = new CompositeTransform();
                        Transforms.Children.Add(_compositeTransform);
                        _compositeTransform.CenterX = (gestureSample.Position.X + gestureSample.Position2.X) / 2;
                        _compositeTransform.CenterY = (gestureSample.Position.Y + gestureSample.Position2.Y) / 2;
                        
                        _gestWidth = Math.Abs(gestureSample.Position.X - gestureSample.Position2.X);
                        _gestHeight = Math.Abs(gestureSample.Position.Y - gestureSample.Position2.Y);

                        _isScaleStarted = true;
                    }
                    else
                    {
                        //if (Transforms.Children.OfType<CompositeTransform>().Select(x=>x.ScaleX).Multiplication() >= 1)
                        {
                            _compositeTransform.ScaleX = Math.Abs(gestureSample.Position.X - gestureSample.Position2.X) / _gestWidth;
                        }

                        //if (Transforms.Children.OfType<CompositeTransform>().Select(x => x.ScaleY).Multiplication() >= 1)
                        {
                            _compositeTransform.ScaleY = Math.Abs(gestureSample.Position.Y - gestureSample.Position2.Y) / _gestHeight;
                        }
                        
                    }
                }
                else if(gestureSample.GestureType == GestureType.PinchComplete)
                {
                    _isScaleStarted = false;
                }
                else if (gestureSample.GestureType == GestureType.VerticalDrag || gestureSample.GestureType == GestureType.HorizontalDrag || gestureSample.GestureType == GestureType.FreeDrag)
                {
                    if(!_isDrugStarted)
                    {
                        _compositeTransform = new CompositeTransform();
                        _drugPosition = gestureSample.Position;
                        Transforms.Children.Add(_compositeTransform);
                        _isDrugStarted = true;
                    }
                    else
                    {
                        _compositeTransform.TranslateX = gestureSample.Position.X - _drugPosition.X;
                        _compositeTransform.TranslateY = gestureSample.Position.Y - _drugPosition.Y;
                    }
                }
                else if (gestureSample.GestureType == GestureType.DragComplete)
                {
                    _isDrugStarted = false;
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