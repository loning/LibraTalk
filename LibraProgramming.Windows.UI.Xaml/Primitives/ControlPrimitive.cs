using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    public class ControlPrimitive : ContentControl
    {
        protected internal const string DisableStateName = "Disabled";
        protected internal const string NoralStateName = "Normal";

        public static readonly DependencyProperty IsFocusedProperty;

        private int visualStateUpdateLock;

        public bool IsLoaded
        {
            get; 
            private set;
        }

        public bool IsTemplateApplied
        {
            get;
            private set;
        }

        public bool IsFocused
        {
            get
            {
                return (bool) GetValue(IsFocusedProperty);
            }
            set
            {
                SetValue(IsFocusedProperty, value);
            }
        }

        protected string CurrentVisualState
        {
            get;
            private set;
        }

        public ControlPrimitive()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            IsEnabledChanged += OnIsEnabledChanged;
        }

        static ControlPrimitive()
        {
            IsFocusedProperty = DependencyProperty
                .Register(
                    "IsFocused",
                    typeof (bool),
                    typeof (ControlPrimitive),
                    new PropertyMetadata(false, OnIsFocusedPropertyChanged)
                );
        }

        public IDisposable BeginUpdateVisualState()
        {
            ++visualStateUpdateLock;
            return new DisposableUpdateLocker(this);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IsTemplateApplied = true;
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;
        }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = false;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            IsFocused = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            IsFocused = false;
        }

        protected internal void UpdateVisualState(bool animate)
        {
            if (!CanUpdateVisualState())
            {
                return;
            }

            var state = GetCurrentVisualStateName();

            if (String.Equals(CurrentVisualState, state, StringComparison.Ordinal))
            {
                return;
            }

            CurrentVisualState = state;

            SetCurrentVisualState(state, animate);
        }

        protected virtual string GetCurrentVisualStateName()
        {
            return IsEnabled ? NoralStateName : DisableStateName;
        }

        protected virtual bool CanUpdateVisualState()
        {
            return IsTemplateApplied && 0 == visualStateUpdateLock;
        }

        protected void EndUpdateVisualState(bool animate)
        {
            if (0 < visualStateUpdateLock)
            {
                visualStateUpdateLock--;
            }

            if (0 == visualStateUpdateLock)
            {
                UpdateVisualState(animate);
            }
        }

        protected TPart GetTemplatePart<TPart>(string partName, bool throwException = true)
            where TPart : class
        {
            var part = GetTemplateChild(partName) as TPart;

            if (null == part && throwException)
            {
                throw new MissingTemplatePartException(typeof (TPart), partName);
            }

            return part;
        }

        private void SetCurrentVisualState(string states, bool animate)
        {
            foreach (var state in states.Split(','))
            {
                VisualStateManager.GoToState(this, state, animate);
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState(IsLoaded);
        }

        private static void OnIsFocusedPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var control = (ControlPrimitive) source;
            control.UpdateVisualState(control.IsLoaded);
        }

        /// <summary>
        /// 
        /// </summary>
        private class DisposableUpdateLocker : IDisposable
        {
            private readonly ControlPrimitive owner;

            public DisposableUpdateLocker(ControlPrimitive owner)
            {
                this.owner = owner;
            }

            public void Dispose()
            {
                owner.EndUpdateVisualState(true);
            }
        }
    }
}
