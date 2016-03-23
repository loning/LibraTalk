using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
    public enum Orientation
    {
        None,
        Landscape,
        Portrait
    }

    public class OrientationStateTrigger : CustomStateTrigger
    {
        public static readonly DependencyProperty OrientationProperty;

        public Orientation Orientation
        {
            get
            {
                return (Orientation) GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        public OrientationStateTrigger()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                var info = DisplayInformation.GetForCurrentView();

                WeakEventListener
                    .AttachEvent<DisplayInformation, object>(
                        handler => info.OrientationChanged += handler,
                        handler => info.OrientationChanged -= handler,
                        OnDisplayOrientationChanged
                    );
            }
        }

        static OrientationStateTrigger()
        {
            OrientationProperty = DependencyProperty
                .Register(
                    "Orientation",
                    typeof (Orientation),
                    typeof (OrientationStateTrigger),
                    new PropertyMetadata(Orientation.None, OnOrientationPropertyChanged)
                );
        }

        private void UpdateTrigger(Orientation value)
        {
            var orientation = DisplayInformation.GetForCurrentView().CurrentOrientation;

            switch (orientation)
            {
                case DisplayOrientations.None:
                    IsActive = false;
                    break;

                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                    IsActive = Orientation.Landscape == value;
                    break;

                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                    IsActive = Orientation.Portrait == value;
                    break;
            }
        }

        private void OnDisplayOrientationChanged(DisplayInformation sender, object e)
        {
            UpdateTrigger(Orientation);
        }

        private static void OnOrientationPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((OrientationStateTrigger) source).UpdateTrigger((Orientation) e.NewValue);
        }
    }
}