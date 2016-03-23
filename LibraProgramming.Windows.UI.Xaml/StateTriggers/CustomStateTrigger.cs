using System;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
	public class CustomStateTrigger : StateTriggerBase, ITriggerValue
	{
		private bool _isActive;

		public bool IsActive
		{
			get
			{
				return _isActive;
			}
			set
			{
				if (value == _isActive)
				{
					return;
				}

				_isActive = value;
				SetActive(value);

                DoSetActive(value);
				DoIsActiveChanged(EventArgs.Empty);
			}
		}

		public event EventHandler IsActiveChanged;

        protected virtual void DoSetActive(bool value)
        {
        }

        protected virtual void DoIsActiveChanged(EventArgs e)
        {
	        IsActiveChanged?.Invoke(this, e);
        }
    }
}