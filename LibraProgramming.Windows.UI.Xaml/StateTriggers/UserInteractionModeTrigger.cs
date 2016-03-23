using System;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
	public class UserInteractionModeTrigger : StateTriggerBase, ITriggerValue
	{
		public static readonly DependencyProperty InteractionModeProperty;

		private bool isActive;

		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				if (value == isActive)
				{
					return;
				}

				isActive = value;
				SetActive(value);

				DoIsActiveChanged(EventArgs.Empty);
			}
		}

        public UserInteractionMode InteractionMode
        {
            get
            {
                return (UserInteractionMode)GetValue(InteractionModeProperty);
            }
            set
            {
                SetValue(InteractionModeProperty, value);
            }
        }

        public event EventHandler IsActiveChanged;

		static UserInteractionModeTrigger()
		{
			InteractionModeProperty = DependencyProperty
				.Register(
					"InteractionMode",
					typeof (UserInteractionMode),
					typeof (UserInteractionModeTrigger),
					new PropertyMetadata(UserInteractionMode.Mouse, OnInteractionModePropertyChanged)
				);
		}

		public UserInteractionModeTrigger()
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

		private void DoIsActiveChanged(EventArgs e)
        {
            IsActiveChanged?.Invoke(this, e);
        }

		private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
		{
			UpdateTrigger(InteractionMode);
		}

		private void UpdateTrigger(UserInteractionMode value)
		{
			IsActive = UIViewSettings.GetForCurrentView().UserInteractionMode == value;
		}

		private static void OnInteractionModePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			((UserInteractionModeTrigger) source).UpdateTrigger((UserInteractionMode) e.NewValue);
		}
	}
}