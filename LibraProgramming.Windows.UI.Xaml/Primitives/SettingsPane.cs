using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
/*
    /// <summary>
    /// 
    /// </summary>
    public enum SettingsPaneLocation
    {
        /// <summary>
        /// 
        /// </summary>
        Right,

        /// <summary>
        /// 
        /// </summary>
        Left
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FlyoutWidth
    {
        /// <summary>
        /// 
        /// </summary>
        Narrow = 346,

        /// <summary>
        /// 
        /// </summary>
        Wide = 646
    }

    /// <summary>
    /// 
    /// </summary>
    public class BackButtonClickedEventArgs : EventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }

        internal BackButtonClickedEventArgs(bool cancel)
        {
            Cancel = cancel;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = BackButtonPartName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PaneBorderPartName, Type = typeof(Border))]
    [TemplatePart(Name = ContentGridPartName, Type = typeof(Grid))]
    public sealed class SettingsPane : ContentControlPrimitive
    {
        private const string BackButtonPartName = "PART_BackButton";
        private const string PaneBorderPartName = "PART_Border";
        private const string ContentGridPartName = "PART_Content";
        private const string ContentScrollerPartName = "PART_ContentScroller";

        public static readonly DependencyProperty EdgeProperty;
        public static readonly DependencyProperty FlyoutWidthProperty;
        public static readonly DependencyProperty IsOpenProperty;

        private const int HorizontalContentOffset = 100;

        private ButtonBase backButton;
        private Grid contentGrid;
        private Border border;
        private readonly Popup hostPopup;
        private ScrollViewer contentScroller;
        private Rect windowBounds;
        private double width;

        /// <summary>
        /// 
        /// </summary>
        public SettingsPaneLocation Edge
        {
            get
            {
                return (SettingsPaneLocation) GetValue(EdgeProperty);
            }
            set
            {
                SetValue(EdgeProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FlyoutWidth FlyoutWidth
        {
            get
            {
                return (FlyoutWidth) GetValue(FlyoutWidthProperty);
            }
            set
            {
                SetValue(FlyoutWidthProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return (bool) GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<BackButtonClickedEventArgs> BackButtonClicked;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<object> Closed;

        public SettingsPane()
        {
            windowBounds = Window.Current.Bounds;

            DefaultStyleKey = typeof(SettingsPane);

            hostPopup = new Popup
            {
                ChildTransitions = new TransitionCollection
                {
                    new PaneThemeTransition
                    {
                        Edge = SettingsPaneLocation.Right == Edge
                            ? EdgeTransitionLocation.Right
                            : EdgeTransitionLocation.Left
                    }
                },
                IsLightDismissEnabled = true,
                Height = windowBounds.Height,
                Child = this
            };

            hostPopup.Closed += OnPopupClosed;

            Canvas.SetTop(hostPopup, 0);

            Height = windowBounds.Height;
        }

        static SettingsPane()
        {
            EdgeProperty = DependencyProperty
                .Register(
                    "Edge",
                    typeof (SettingsPaneLocation),
                    typeof (SettingsPane),
                    new PropertyMetadata(SettingsPaneLocation.Right, OnEdgePropertyChanged)
                );
            FlyoutWidthProperty = DependencyProperty
                .Register(
                    "FlyoutWidth",
                    typeof (FlyoutWidth),
                    typeof (SettingsPane),
                    new PropertyMetadata(FlyoutWidth.Narrow)
                );
            IsOpenProperty = DependencyProperty
                .Register(
                    "IsOpen",
                    typeof (bool),
                    typeof (SettingsPane),
                    new PropertyMetadata(false, OnIsOpenPropertyChanged)
                );
        }

        protected override void OnApplyTemplate()
        {
            if (null != backButton)
            {
                backButton.Click -= OnBackButtonClick;
            }

            backButton = GetTemplatePart<ButtonBase>(BackButtonPartName, false);

            if (null != backButton)
            {
                backButton.Click += OnBackButtonClick;
            }

            contentGrid = GetTemplatePart<Grid>(ContentGridPartName);

            contentGrid.Transitions = new TransitionCollection
            {
                new EntranceThemeTransition
                {
                    FromHorizontalOffset =
                        SettingsPaneLocation.Right == Edge ? HorizontalContentOffset : -HorizontalContentOffset
                }
            };

            border = GetTemplatePart<Border>(PaneBorderPartName);
            contentScroller = GetTemplatePart<ScrollViewer>(ContentScrollerPartName);

            base.OnApplyTemplate();
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.Activated += OnWindowCurrentActivated;
            Window.Current.SizeChanged += OnWindowCurrentSizeChanged;

            if (SettingsPaneLocation.Left == Edge)
            {
                border.BorderThickness = new Thickness(0, 0, 1, 0);
            }

            width = (double) FlyoutWidth;

            hostPopup.Width = width;
            contentScroller.Width = width;
            Width = width;

            Canvas.SetLeft(hostPopup, SettingsPaneLocation.Right == Edge ? windowBounds.Width - width : 0);

            if (null == hostPopup.Parent)
            {
                //var pane=Windows.UI.
            }

            base.OnLoaded(sender, e);
        }

        private void OnPopupClosed(object sender, object e)
        {
            hostPopup.Child = null;

            Window.Current.Activated -= OnWindowCurrentActivated;
            Window.Current.SizeChanged -= OnWindowCurrentSizeChanged;

            Content = null;

            Closed?.Invoke(this, e);

            IsOpen = false;
        }

        private void DoBackButtonClicked(BackButtonClickedEventArgs arg)
        {
            var handler = BackButtonClicked;
            handler?.Invoke(this, arg);
        }
        
        private void OnWindowCurrentActivated(object sender, WindowActivatedEventArgs e)
        {
            if (CoreWindowActivationState.Deactivated == e.WindowActivationState)
            {
                IsOpen = false;
            }
        }

        private void OnWindowCurrentSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            IsOpen = false;
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            var arg = new BackButtonClickedEventArgs(false);

            DoBackButtonClicked(arg);

            if (arg.Cancel)
            {
                return;
            }

            if (null != hostPopup)
            {
                hostPopup.IsOpen = false;
            }

            if (ApplicationViewState.Snapped == ApplicationView.Value)
            {
                ;
            }
        }

        private void OnEdgeChanged(SettingsPaneLocation value)
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            var transition = (EntranceThemeTransition) contentGrid.Transitions[0];

            transition.FromHorizontalOffset = SettingsPaneLocation.Right == value
                ? HorizontalContentOffset
                : -HorizontalContentOffset;
        }

        private void OnIsOpenChanged(bool previos, bool current)
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            if (previos == current)
            {
                return;
            }

            hostPopup.IsOpen = current;
        }

        private static void OnEdgePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SettingsPane) source).OnEdgeChanged((SettingsPaneLocation) e.NewValue);
        }

        private static void OnIsOpenPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SettingsPane) source).OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
        }
    }
*/
}
