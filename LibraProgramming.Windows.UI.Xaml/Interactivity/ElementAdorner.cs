using System;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.Interactivity
{
    public abstract class ElementAdorner : DependencyObject, IElementAdorner
    {
        public FrameworkElement Element
        {
            get;
            private set;
        }

        public void Attach(FrameworkElement element)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (null != Element)
            {
                throw new InvalidOperationException();
            }

            Element = element;

            DoAttach();
        }

        public void Release()
        {
            if (null == Element)
            {
                throw new InvalidOperationException();
            }

            Element = null;

            DoRelease();
        }

        protected abstract void DoAttach();

        protected abstract void DoRelease();
    }
}