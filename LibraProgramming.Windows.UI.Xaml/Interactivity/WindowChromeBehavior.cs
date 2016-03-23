using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace LibraProgramming.Windows.UI.Xaml.Interactivity
{
    public sealed class WindowChromeBehavior : DependencyObject, IBehavior
    {
        public static readonly DependencyProperty TitleBarColorActionProperty;
        public static readonly DependencyProperty AssociatedObjectProperty;

        /// <summary>
        /// Получает объект <see cref="T:Windows.UI.Xaml.DependencyObject"/>, к которому прикреплен объект <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/>.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get
            {
                return (DependencyObject) GetValue(AssociatedObjectProperty);
            }
            private set
            {
                SetValue(AssociatedObjectProperty, value);
            }
        }

        public ITitleBarColorAction TitleBarColorAction
        {
            get
            {
                return (ITitleBarColorAction) GetValue(TitleBarColorActionProperty);
            }
            set
            {
                SetValue(TitleBarColorActionProperty, value);
            }
        }

        static WindowChromeBehavior()
        {
            TitleBarColorActionProperty = DependencyProperty
                .Register(
                    "TitleBarColorAction",
                    typeof (Color),
                    typeof (WindowChromeBehavior),
                    new PropertyMetadata(null)
                );
            AssociatedObjectProperty = DependencyProperty
                .Register(
                    "AssociatedObject",
                    typeof (DependencyObject),
                    typeof (WindowChromeBehavior),
                    new PropertyMetadata(null)
                );
        }

        /// <summary>
        /// Прикрепляет к заданному объекту.
        /// </summary>
        /// <param name="associatedObject">Тип <see cref="T:Windows.UI.Xaml.DependencyObject"/>, к которому будет прикреплен тип <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/>.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if (null == associatedObject)
            {
                throw new ArgumentNullException(nameof(associatedObject));
            }

            if (null != AssociatedObject)
            {
                throw new ArgumentException("", nameof(associatedObject));
            }

            var element = associatedObject as FrameworkElement;

            if (null != element)
            {
                element.Loaded += OnElementLoaded;
            }

            AssociatedObject = associatedObject;
        }

        /// <summary>
        /// Отсоединяет этот экземпляр от связанного с ним объекта.
        /// </summary>
        public void Detach()
        {
            if (null == AssociatedObject)
            {
                throw new InvalidOperationException();
            }

            AssociatedObject = null;
        }

        private void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            var action = TitleBarColorAction;

            if (null == action)
            {
                return;
            }

            var appview = ApplicationView.GetForCurrentView();

            if (null == appview)
            {
                return;
            }

            action.Execute(this, appview.TitleBar);
        }
    }
}