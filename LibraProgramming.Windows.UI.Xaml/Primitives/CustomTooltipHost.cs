namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
//    [TemplatePart(Type = typeof(Popup), Name = PopupHostPartName)]
    internal sealed class CustomTooltipHost : ContentControlPrimitive
    {
//        private const string PopupHostPartName = "PART_PopupHost";

//        public static readonly DependencyProperty PlacementProperty;
//        public static readonly DependencyProperty PlacementTargetProperty;
//        public static readonly DependencyProperty IsOpenProperty;

//        private Popup popupHost;

        /*public bool IsOpen
        {
            get
            {
                return (bool) GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }*/

/*
        public PlacementMode Placement
        {
            get
            {
                return (PlacementMode) GetValue(PlacementProperty);
            }
            set
            {
                SetValue(PlacementProperty, value);
            }
        }
*/

/*
        public UIElement PlacementTarget
        {
            get
            {
                return (UIElement)GetValue(PlacementTargetProperty);
            }
            set
            {
                SetValue(PlacementTargetProperty, value);
            }
        }
*/

        public CustomTooltipHost()
        {
            DefaultStyleKey = typeof (CustomTooltipHost);
        }

/*
        static CustomTooltipHost()
        {
            IsOpenProperty = DependencyProperty
                .Register(
                    "IsOpen",
                    typeof (bool),
                    typeof (CustomTooltipHost),
                    new PropertyMetadata(false, OnIsOpenPropertyChanged)
                );
            PlacementProperty = DependencyProperty
                .Register(
                    "Placement",
                    typeof (PlacementMode),
                    typeof (CustomTooltipHost),
                    new PropertyMetadata(PlacementMode.Mouse, OnPlacementPropertyChanged)
                );
            PlacementTargetProperty = DependencyProperty
                .Register(
                    "PlacementTarget",
                    typeof(UIElement),
                    typeof(CustomTooltipHost),
                    new PropertyMetadata(null, OnPlacementTargetPropertyChanged)
                );
        }
*/

        protected override void OnApplyTemplate()
        {
            /*if (null != popupHost)
            {
//                popupHost.Loaded -= OnPopupLoaded;
                popupHost.Opened -= OnPopupOpened;
                popupHost.Closed -= OnPopupClosed;
            }

            popupHost = GetTemplatePart<Popup>(PopupHostPartName);

            popupHost.Closed += OnPopupClosed;
            popupHost.Opened += OnPopupOpened;
//            popupHost.Loaded += OnPopupLoaded;
*/
            base.OnApplyTemplate();

            UpdateVisualState(false);
        }

        /*private void OnPopupLoaded(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }*/

/*
        private void OnPopupOpened(object sender, object e)
        {
//            throw new System.NotImplementedException();
        }
*/

/*
        private void OnPopupClosed(object sender, object e)
        {
//            throw new System.NotImplementedException();
        }
*/

/*
        private void OnIsOpenedChanged(bool value)
        {
            if (value)
            {
//                ApplyTemplate();
            }
        }
*/

/*
        private static void OnIsOpenPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CustomTooltipHost) source).OnIsOpenedChanged((bool) e.NewValue);
        }
*/

/*
        private static void OnPlacementPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
//            throw new System.NotImplementedException();
        }
*/

/*
        private static void OnPlacementTargetPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            //throw new System.NotImplementedException();
        }
*/
    }
}