using Windows.System.Profile;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
    /// <summary>
    /// Device families.
    /// </summary>
	public enum DeviceFamily
	{
        /// <summary>
        /// Unknown device type.
        /// </summary>
		Unknown,

        /// <summary>
        /// Desktop computers.
        /// </summary>
        Desktop,

        /// <summary>
        /// Any mobile devices.
        /// </summary>
        Mobile,
        Team,

        /// <summary>
        /// Internet of Things devices.
        /// </summary>
        IoT,

        /// <summary>
        /// Xbox device family.
        /// </summary>
        Xbox
	}

	public sealed class DeviceFamilyStateTrigger : CustomStateTrigger
	{
		public static readonly DependencyProperty DeviceFamilyProperty;

		private static readonly string devicefamily;

		public DeviceFamily DeviceFamily
		{
			get
			{
				return (DeviceFamily) GetValue(DeviceFamilyProperty);
			}
			set
			{
				SetValue(DeviceFamilyProperty, value);
			}
		}

		static DeviceFamilyStateTrigger()
		{
			devicefamily = AnalyticsInfo.VersionInfo.DeviceFamily;
			DeviceFamilyProperty = DependencyProperty
				.Register(
					"DeviceFamily",
					typeof (DeviceFamily),
					typeof (DeviceFamilyStateTrigger),
					new PropertyMetadata(DeviceFamily.Unknown, OnDeviceFamilyPropertyChanged)
				);
		}

		private void OnDeviceFamilyChanged(DeviceFamily value)
		{
			switch (devicefamily)
			{
                case "Windows.Mobile":
					IsActive = DeviceFamily.Mobile == value;
                    break;

                case "Windows.Desktop":
					IsActive = DeviceFamily.Desktop == value;
                    break;

                case "Windows.Team":
					IsActive = DeviceFamily.Team == value;
                    break;

                case "Windows.IoT":
					IsActive = DeviceFamily.IoT == value;
                    break;

                case "Windows.Xbox":
					IsActive = DeviceFamily.Xbox == value;
                    break;

                default:
					IsActive = DeviceFamily.Unknown == value;
                    break;
			}
		}

		private static void OnDeviceFamilyPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
	        ((DeviceFamilyStateTrigger) source).OnDeviceFamilyChanged((DeviceFamily) e.NewValue);
        }
	}
}