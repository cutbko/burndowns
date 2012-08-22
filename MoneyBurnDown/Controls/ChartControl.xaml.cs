using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Input.Touch;
using MoneyBurnDown.Model.Entities;

namespace MoneyBurnDown.Controls
{
    public partial class ChartControl
    {
        public ChartControl()
        {
            InitializeComponent();
            Touch.FrameReported += TouchFrameReported;
            TouchPanel.EnabledGestures = GestureType.DoubleTap;
        }

        #region REWRITE THIS!
        void TouchFrameReported(object sender, TouchFrameEventArgs e)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                if(TouchPanel.ReadGesture().GestureType==GestureType.DoubleTap)
                {
                    OnDoubleTapped(EventArgs.Empty);
                }
            }
        }

        public event EventHandler DoubleTapped;

        private void OnDoubleTapped(EventArgs e)
        {
            EventHandler handler = DoubleTapped;
            if (handler != null) handler(this, e);
        }
        #endregion

        public Burndown Burndown
        {
            get { return (Burndown)GetValue(BurndownProperty); }
            set
            {
                SetValue(BurndownProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BurndownProperty =
            DependencyProperty.Register("Burndown", typeof(Burndown), typeof(ChartControl), new PropertyMetadata(null));

        protected override Size MeasureOverride(Size availableSize)
        {
            CreateGraph(availableSize);
            return base.MeasureOverride(availableSize);
        }

        private void CreateGraph(Size availableSize)
        {
            LayoutRoot.Children.Clear();

            Brush graphBrush = new SolidColorBrush(Colors.Red);
            Brush defaultBurndownBrush = new SolidColorBrush(Colors.Green);
            Brush userBurndownBrush = new SolidColorBrush(Colors.Blue);
            Brush nowBurndownBrush = new SolidColorBrush(Colors.Gray);

            AddLine(graphBrush, 0, 0, 0, availableSize.Height);
            AddLine(graphBrush, 0, availableSize.Height, availableSize.Width, availableSize.Height);
            AddLine(defaultBurndownBrush, 0, 0, availableSize.Width, availableSize.Height);
           
            if (Burndown == null)
            {
                return;
            }

            var daywithmoney = Burndown.Transactions.Select(x => new { x.CreatedAt.Date, Amount= (double)x.Amount }).GroupBy(x => x.Date).Select(x => new { Date = x.Key, Money = x.Sum(u => u.Amount) }).ToArray();
            
            var drawableItems = Enumerable.Range(0, (int) Math.Ceiling((Burndown.EndDate.Date - Burndown.StartDate.Date).TotalDays)).Select(x => new
                                                         {
                                                             Date = Burndown.StartDate.Date.AddDays(x),
                                                             Amount = daywithmoney.Where(item => item.Date == Burndown.StartDate.Date.AddDays(x)).Select(item => item.Money).SingleOrDefault()
                                                         }).ToList();

            double widthPerItem = availableSize.Width/drawableItems.Count;

            DateTime now = DateTime.Now;
            if (now < Burndown.EndDate && now > Burndown.StartDate)
            {
                double burnDays = (now - Burndown.StartDate.Date).TotalDays;
                double curX = widthPerItem * burnDays;

                AddLine(nowBurndownBrush, curX, 0, curX, availableSize.Height);

            }

            double heightPerUnit = availableSize.Height / (double)Burndown.MoneyOnStart;

            double x1 = 0, x2 = 0, y1 = 0, y2 = 0;
            double amountOnStart;
            double currentAmount = amountOnStart = (double) Burndown.MoneyOnStart;

            for (int index = 0; index < drawableItems.Count; index++)
            {
                if (drawableItems.Skip(index).All(x => Math.Abs(x.Amount) == 0))
                {
                    if (index == 0)
                    {
                        return;
                    }

                    x1 = x2;
                    y1 = y2;

                    x2 = availableSize.Width;
                    y2 = y1 * x2 / x1;

                    AddLine(userBurndownBrush, x1, y1, x2, y2, 5, new DoubleCollection {3, 3});
                    return;
                }

                var item = drawableItems[index];
                x2 = x1 + widthPerItem;
                currentAmount -= item.Amount;
                y2 = (amountOnStart - currentAmount)*heightPerUnit;
                AddLine(userBurndownBrush, x1, y1, x2, y2, 5);
                x1 = x2;
                y1 = y2;
            }
        }

        private void AddLine(Brush brush, double x1, double y1, double x2, double y2, double strokeThickness = 2, DoubleCollection doubleCollection = null)
        {
            LayoutRoot.Children.Add(new Line
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                Stroke = brush,
                StrokeThickness = strokeThickness,
                StrokeDashArray = doubleCollection
            });
        }
    }
}
