using System.Windows;
using System.Windows.Controls;

namespace ChatApp.View.Utilities
{
    public static class ScrollHelper
    {
        public static readonly DependencyProperty AutoScrollToEndProperty = DependencyProperty.RegisterAttached(
            "AutoScrollToEnd", typeof(bool), typeof(ScrollHelper), new PropertyMetadata(false, OnAutoScrollToEndChanged));

        public static bool GetAutoScrollToEnd(ScrollViewer scrollViewer)
        {
            return (bool)scrollViewer.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(ScrollViewer scrollViewer, bool value)
        {
            scrollViewer.SetValue(AutoScrollToEndProperty, value);
        }

        private static void OnAutoScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                bool autoScrollToEnd = (bool)e.NewValue;
                if (autoScrollToEnd)
                {
                    scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                }
                else
                {
                    scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                }
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (e.ExtentHeightChange > 0)
            {
                scrollViewer.ScrollToEnd();
            }
        }
    }
}