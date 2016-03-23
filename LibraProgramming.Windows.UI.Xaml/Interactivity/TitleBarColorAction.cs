using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.Interactivity
{
    public class TitleBarColorAction : DependencyObject, ITitleBarColorAction
    {
        public static readonly DependencyProperty BackgroundColorProperty;
        public static readonly DependencyProperty ForegroundColorProperty;
        public static readonly DependencyProperty InactiveBackgroundColorProperty;
        public static readonly DependencyProperty InactiveForegroundColorProperty;
        public static readonly DependencyProperty ButtonBackgroundColorProperty;
        public static readonly DependencyProperty ButtonForegroundColorProperty;
        public static readonly DependencyProperty ButtonHoverBackgroundColorProperty;
        public static readonly DependencyProperty ButtonHoverForegroundColorProperty;
        public static readonly DependencyProperty ButtonInactiveBackgroundColorProperty;
        public static readonly DependencyProperty ButtonInactiveForegroundColorProperty;

        public Color BackgroundColor
        {
            get
            {
                return (Color) GetValue(BackgroundColorProperty);
            }
            set
            {
                SetValue(BackgroundColorProperty, value);
            }
        }

        public Color ForegroundColor
        {
            get
            {
                return (Color) GetValue(ForegroundColorProperty);
            }
            set
            {
                SetValue(ForegroundColorProperty, value);
            }
        }

        public Color InactiveBackgroundColor
        {
            get
            {
                return (Color) GetValue(InactiveBackgroundColorProperty);
            }
            set
            {
                SetValue(InactiveBackgroundColorProperty, value);
            }
        }

        public Color InactiveForegroundColor
        {
            get
            {
                return (Color) GetValue(InactiveForegroundColorProperty);
            }
            set
            {
                SetValue(InactiveForegroundColorProperty, value);
            }
        }

        public Color ButtonBackgroundColor
        {
            get
            {
                return (Color) GetValue(ButtonBackgroundColorProperty);
            }
            set
            {
                SetValue(ButtonBackgroundColorProperty, value);
            }
        }

        public Color ButtonForegroundColor
        {
            get
            {
                return (Color) GetValue(ButtonForegroundColorProperty);
            }
            set
            {
                SetValue(ButtonForegroundColorProperty, value);
            }
        }

        public Color ButtonHoverBackgroundColor
        {
            get
            {
                return (Color) GetValue(ButtonHoverBackgroundColorProperty);
            }
            set
            {
                SetValue(ButtonHoverBackgroundColorProperty, value);
            }
        }

        public Color ButtonHoverForegroundColor
        {
            get
            {
                return (Color) GetValue(ButtonHoverForegroundColorProperty);
            }
            set
            {
                SetValue(ButtonHoverForegroundColorProperty, value);
            }
        }

        public Color ButtonInactiveBackgroundColor
        {
            get
            {
                return (Color) GetValue(ButtonInactiveBackgroundColorProperty);
            }
            set
            {
                SetValue(ButtonInactiveBackgroundColorProperty, value);
            }
        }

        public Color ButtonInactiveForegroundColor
        {
            get
            {
                return (Color) GetValue(ButtonInactiveForegroundColorProperty);
            }
            set
            {
                SetValue(ButtonInactiveForegroundColorProperty, value);
            }
        }

        static TitleBarColorAction()
        {
            BackgroundColorProperty = DependencyProperty
                .Register(
                    "BackgroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ForegroundColorProperty = DependencyProperty
                .Register(
                    "ForegroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            InactiveBackgroundColorProperty = DependencyProperty
                .Register(
                    "InactiveBackgroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            InactiveForegroundColorProperty = DependencyProperty
                .Register(
                    "InactiveForegroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ButtonBackgroundColorProperty = DependencyProperty
                .Register(
                    "ButtonBackgroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ButtonForegroundColorProperty = DependencyProperty
                .Register(
                    "ButtonForegroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ButtonHoverBackgroundColorProperty = DependencyProperty
                .Register(
                    "ButtonHoverBackgroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ButtonHoverForegroundColorProperty = DependencyProperty
                .Register(
                    "ButtonHoverForegroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ButtonInactiveBackgroundColorProperty = DependencyProperty
                .Register(
                    "ButtonInactiveBackgroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ButtonInactiveForegroundColorProperty = DependencyProperty
                .Register(
                    "ButtonInactiveForegroundColor",
                    typeof (Color),
                    typeof (TitleBarColorAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        /// <summary>
        /// Выполняет действие.
        /// </summary>
        /// <param name="sender">Объект <see cref="T:System.Object"/>, который передается действию поведением. Как правило, это свойство <seealso cref="P:Microsoft.Xaml.Interactivity.IBehavior.AssociatedObject"/> или целевой объект.</param><param name="parameter">Значение данного параметра определяется вызывающим объектом.</param>
        /// <remarks>
        /// Пример использования параметра — поведение EventTriggerBehavior, передающее аргументы EventArgs в качестве параметра его действиям.
        /// </remarks>
        /// <returns>
        /// Возвращает результат этого действия.
        /// </returns>
        public object Execute(object sender, object parameter)
        {
            var titlebar = parameter as ApplicationViewTitleBar;

            if (null == titlebar)
            {
                throw new InvalidCastException();
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(BackgroundColorProperty))
            {
                titlebar.BackgroundColor = BackgroundColor;
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(ForegroundColorProperty))
            {
                titlebar.ForegroundColor = ForegroundColor;
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(InactiveBackgroundColorProperty))
            {
                titlebar.InactiveBackgroundColor = InactiveBackgroundColor;
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(InactiveForegroundColorProperty))
            {
                titlebar.InactiveForegroundColor = InactiveForegroundColor;
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(ButtonBackgroundColorProperty))
            {
                titlebar.ButtonBackgroundColor = ButtonBackgroundColor;
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(ButtonForegroundColorProperty))
            {
                titlebar.ButtonForegroundColor = ButtonForegroundColor;
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(ButtonHoverBackgroundColorProperty))
            {
                titlebar.ButtonHoverBackgroundColor = ButtonHoverBackgroundColor;
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(ButtonHoverForegroundColorProperty))
            {
                titlebar.ButtonHoverForegroundColor = ButtonHoverForegroundColor;
            }

            if (DependencyProperty.UnsetValue != ReadLocalValue(ButtonInactiveBackgroundColorProperty))
            {
                titlebar.ButtonInactiveBackgroundColor = ButtonInactiveBackgroundColor;
            }

            return parameter;
        }
    }
}