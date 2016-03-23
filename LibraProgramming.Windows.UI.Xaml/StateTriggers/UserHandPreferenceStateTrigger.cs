using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
    /// <summary>
    /// Trigger for switching UI based on whether the user favours their left or right hand.
    /// </summary>
	public class UserHandPreferenceStateTrigger:StateTriggerBase,ITriggerValue
	{
		public static readonly DependencyProperty HandPreferenceProperty;

		private static HandPreference handPreference;

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

                isActive = true;
                SetActive(value);

                DoIsActiveChanged(EventArgs.Empty);
            }
        }

        public HandPreference HandPreference
        {
            get
            {
                return (HandPreference)GetValue(HandPreferenceProperty);
            }
            set
            {
                SetValue(HandPreferenceProperty, value);
            }
        }

        public event EventHandler IsActiveChanged;

		static UserHandPreferenceStateTrigger()
		{
			handPreference = new UISettings().HandPreference;
            HandPreferenceProperty = DependencyProperty
				.Register(
					"HandPreference",
					typeof (HandPreference),
					typeof (UserHandPreferenceStateTrigger),
					new PropertyMetadata(HandPreference.RightHanded, OnHandPreferencePropertyChanged)
				);
		}

		private void DoIsActiveChanged(EventArgs e)
        {
            IsActiveChanged?.Invoke(this, e);
        }

		private void OnHandPreferenceChanged(HandPreference value)
		{
			IsActive = handPreference == value;
		}

		private static void OnHandPreferencePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
	        ((UserHandPreferenceStateTrigger) source).OnHandPreferenceChanged((HandPreference) e.NewValue);
        }
	}
}