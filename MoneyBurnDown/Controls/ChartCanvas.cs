using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Input.Touch;
using MoneyBurnDown.Model.Entities;

namespace MoneyBurnDown.Controls
{
    public class ChartCanvas : Canvas
    {
        private Size? _lastSize;

        public ChartCanvas()
        {
            SizeChanged += ChartCanvasSizeChanged;
            Touch.FrameReported += TouchFrameReported;
        }

        #region REWRITE THIS!
        void TouchFrameReported(object sender, TouchFrameEventArgs e)
        {
            if(!AreGesturesEnabled)
            {
                return;
            }

            while (TouchPanel.IsGestureAvailable)
            {
                if (TouchPanel.ReadGesture().GestureType == GestureType.DoubleTap)
                {
                    OnDoubleTapped(EventArgs.Empty);
                }
            }
        }
        #endregion
        
        public bool AreGesturesEnabled
        {
            get { return (bool)GetValue(AreGesturesEnabledProperty); }
            set { SetValue(AreGesturesEnabledProperty, value); }
        }

        public static readonly DependencyProperty AreGesturesEnabledProperty =
            DependencyProperty.Register("AreGesturesEnabled", typeof(bool), typeof(ChartCanvas), new PropertyMetadata(false));

        public event EventHandler DoubleTapped;

        private void OnDoubleTapped(EventArgs e)
        {
            EventHandler handler = DoubleTapped;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public bool AreNumbersEnabled
        {
            get { return (bool)GetValue(AreNumbersEnabledProperty); }
            set { SetValue(AreNumbersEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AreNumbersEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AreNumbersEnabledProperty =
            DependencyProperty.Register("AreNumbersEnabled", typeof(bool), typeof(ChartCanvas), new PropertyMetadata(false, (o, args) => ((ChartCanvas)o).Create()));

        void ChartCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _lastSize = e.NewSize;
            Create();
        }

        private void Create()
        {
            if (_lastSize.HasValue)
            {
                CreateGraph(AreNumbersEnabled
                                ? new Rect(10, 0, _lastSize.Value.Width - 10, _lastSize.Value.Height - 30)
                                : new Rect(0, 0, _lastSize.Value.Width, _lastSize.Value.Height));
            }
        }

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
            DependencyProperty.Register("Burndown", typeof(Burndown), typeof(ChartCanvas), new PropertyMetadata(null));

        public void CreateGraph(Rect rect)
        {
            Children.Clear();

            Brush graphBrush = new SolidColorBrush(Colors.Red);
            Brush defaultBurndownBrush = new SolidColorBrush(Colors.Green);
            Brush userBurndownBrush = new SolidColorBrush(Colors.Blue);
            Brush nowBurndownBrush = new SolidColorBrush(Colors.Gray);
            Brush dailyUses = new SolidColorBrush(Colors.Yellow);
            Brush gridBrush = new SolidColorBrush(new Color
            {
                R = 230,
                B = 230,
                G = 230,
                A = 50
            });

            AddLine(graphBrush, rect.X, rect.Y, rect.X, rect.Bottom, 4);
            AddLine(graphBrush, rect.X, rect.Bottom, rect.X + rect.Width, rect.Bottom, 4);
            AddLine(defaultBurndownBrush, rect.X, rect.Y, rect.Right, rect.Bottom, 4);

            if (Burndown == null)
            {
                return;
            }

            var daywithmoney = Burndown.Transactions.Select(x => new { x.CreatedAt.Date, x.Amount }).GroupBy(x => x.Date).Select(x => new { Date = x.Key, Money = x.Sum(u => u.Amount) }).ToArray();

            var drawableItems = Enumerable.Range(0, (int)Math.Ceiling((Burndown.EndDate.Date - Burndown.StartDate.Date).TotalDays)).Select(x => new
            {
                Date = Burndown.StartDate.Date.AddDays(x),
                Amount = daywithmoney.Where(item => item.Date == Burndown.StartDate.Date.AddDays(x)).Select(item => item.Money).SingleOrDefault()
            }).ToList();

            double widthPerItem = rect.Width / drawableItems.Count;

            double heightPerUnit = rect.Height / (double)Burndown.MoneyOnStart;

            double x1 = 0, x2 = 0, y1 = 0, y2 = 0;
            decimal amountOnStart;
            decimal currentAmount = amountOnStart = Burndown.MoneyOnStart;

            int koef = 1;

            if (drawableItems.Count > 7)
            {
                koef = (int) Math.Ceiling((double)drawableItems.Count / 7);
            }

            for (int i = 0; i < drawableItems.Count; i += koef)
            {
                double x = i*widthPerItem + rect.X;
                AddLine(gridBrush, x, rect.Y, x, rect.Bottom);

                if (AreNumbersEnabled)
                {
                    AddLine(graphBrush, x, rect.Bottom + 5, x, rect.Bottom - 15, 5);
                    TextBlock textBlock = new TextBlock
                    {
                        Text = drawableItems[i].Date.ToString("dd.MM")
                    };
                    Children.Add(textBlock);
                    Canvas.SetTop(textBlock, rect.Bottom);
                    Canvas.SetLeft(textBlock, rect.X + x - textBlock.ActualWidth/2);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                double height = rect.Height/10*i + rect.Y;
                AddLine(gridBrush, rect.X, height, rect.Right, height);

                if (AreNumbersEnabled)
                {
                    AddLine(graphBrush, rect.X, height, rect.X + 15, height, 5);
                    TextBlock textBlock = new TextBlock
                                              {
                                                  Text = (Burndown.MoneyOnStart / 10 * (10 - i)).ToString("0.00", CultureInfo.InvariantCulture)
                                              };
                    Children.Add(textBlock);
                    Canvas.SetTop(textBlock, height - textBlock.ActualHeight / 2);
                    Canvas.SetLeft(textBlock, rect.Right - textBlock.ActualWidth);
                }
            }

            DateTime now = DateTime.Now;
            if (now < Burndown.EndDate && now > Burndown.StartDate)
            {
                double burnDays = (now - Burndown.StartDate.Date).TotalDays;
                double curX = widthPerItem * burnDays ;

                AddLine(nowBurndownBrush, curX + rect.X, rect.Y, curX + rect.X, rect.Bottom, 3);
            }

            for (int index = 0; index < drawableItems.Count; index++)
            {
                if (drawableItems[index].Date > DateTime.Today && drawableItems.Skip(index).All(x => x.Amount == 0))
                {
                    if (index == 0)
                    {
                        return;
                    }

                    x1 = x2;
                    y1 = y2;

                    x2 = rect.Width;
                    y2 = y1 * x2 / x1;

                    AddLine(userBurndownBrush, x1 + rect.X, y1 + rect.Y, x2 + rect.X, y2 + rect.Y, 5, new DoubleCollection { 3, 3 });
                    return;
                }

                var item = drawableItems[index];
                x2 = x1 + widthPerItem;
                currentAmount -= item.Amount;
                y2 = (double)(amountOnStart - currentAmount) * heightPerUnit;
                AddLine(userBurndownBrush, x1 + rect.X, y1 + rect.Y, x2 + rect.X, y2 + rect.Y, 6);
                if(AreNumbersEnabled)
                {
                    AddLine(dailyUses, x2 + rect.X, rect.Bottom, x2 + rect.X, rect.Bottom - (double)item.Amount * heightPerUnit, 6);
                }

                x1 = x2;
                y1 = y2;
            }
        }

        private void AddLine(Brush brush, double x1, double y1, double x2, double y2, double strokeThickness = 2, DoubleCollection doubleCollection = null)
        {
            Children.Add(new Line
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