using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
	public class AdaptiveSizeStateTrigger : CustomStateTrigger
	{
		public static readonly DependencyProperty MinWindowHeightProperty;
		public static readonly DependencyProperty MinWindowWidthProperty;

		public double MinWindowHeight
		{
			get
			{
				return (double) GetValue(MinWindowHeightProperty);
			}
			set
			{
				SetValue(MinWindowHeightProperty, value);
			}
		}

		public double MinWindowWidth
		{
			get
			{
				return (double) GetValue(MinWindowWidthProperty);
			}
			set
			{
				SetValue(MinWindowWidthProperty, value);
			}
		}

		public AdaptiveSizeStateTrigger()
		{
            if (!DesignMode.DesignModeEnabled)
            {
                WeakEventListener
                    .AttachEvent<object, WindowSizeChangedEventArgs>(
                        handler => Window.Current.SizeChanged += new WindowSizeChangedEventHandler(handler),
                        handler => Window.Current.SizeChanged -= new WindowSizeChangedEventHandler(handler),
                        OnWindowSizeChanged
                    );
            }
        }

		static AdaptiveSizeStateTrigger()
		{
			MinWindowHeightProperty = DependencyProperty
				.Register(
					"MinWindowHeight",
					typeof (double),
					typeof (AdaptiveSizeStateTrigger),
					new PropertyMetadata(Double.NaN, OnMinWindowHeightPropertyChanged)
				);
			MinWindowWidthProperty = DependencyProperty
				.Register(
					"MinWindowWidth",
					typeof (double),
					typeof (AdaptiveSizeStateTrigger),
					new PropertyMetadata(Double.NaN, OnMinWindowWidthPropertyChanged)
				);
		}

		private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs args)
		{
			UpdateTrigger();
		}

		private void UpdateTrigger()
		{
			var bounds = Window.Current.Bounds;
			var value = MinWindowHeight;

            var index = 0;
			var results = new bool[2];

		    if (!Double.IsNaN(value) && 0.0 < value)
		    {
		        results[index++] = bounds.Height >= value;
		    }

		    value = MinWindowWidth;

		    if (!Double.IsNaN(value) && 0.0 < value)
		    {
		        results[index++] = bounds.Width >= value;
		    }

		    IsActive = TrueForAll(results, index);
		}

		private static bool TrueForAll(IReadOnlyList<bool> array, int count)
		{
			for (var index = 0; index < count; index++)
			{
				if (!array[index])
				{
					return false;
				}
			}
            
            return true;
		}

		private static void OnMinWindowHeightPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
	        ((AdaptiveSizeStateTrigger) source).UpdateTrigger();
        }

        private static void OnMinWindowWidthPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
	        ((AdaptiveSizeStateTrigger) source).UpdateTrigger();
        }
    }
}