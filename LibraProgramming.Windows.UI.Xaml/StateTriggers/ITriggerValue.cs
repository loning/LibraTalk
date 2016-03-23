using System;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
	public interface ITriggerValue
	{
		bool IsActive
		{
			get;
		}

		event EventHandler IsActiveChanged;
	}
}