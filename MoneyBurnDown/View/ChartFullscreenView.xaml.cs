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
using MoneyBurnDown.ViewModel;

namespace MoneyBurnDown.View
{
    public partial class ChartFullscreenView : PhoneApplicationPage
    {
        public ChartFullscreenView()
        {
            InitializeComponent();
        }

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