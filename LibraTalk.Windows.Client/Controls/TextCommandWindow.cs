using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using LibraProgramming.Windows.UI.Xaml.Primitives;

namespace LibraTalk.Windows.Client.Controls
{
    [TemplatePart(Type = typeof(TextBlock), Name = HistoryTextBlockPartName)]
    [TemplatePart(Type = typeof(TextBox), Name = InputTextBoxPartName)]
    public sealed class TextCommandWindow : ControlPrimitive
    {
        private const string HistoryTextBlockPartName = "PART_HistoryTextBlock";
        private const string InputTextBoxPartName = "PART_InputTextBox";

        private TextBlock history;
        private TextBox input;

        public TextCommandWindow()
        {
            DefaultStyleKey = typeof (TextCommandWindow);
        }

        protected override void OnApplyTemplate()
        {
            if (null != history)
            {
                
            }

            if (null != input)
            {
                
            }

            history = GetTemplatePart<TextBlock>(HistoryTextBlockPartName);
            input = GetTemplatePart<TextBox>(InputTextBoxPartName);
            
            base.OnApplyTemplate();
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += OnCoreWindowKeyDown;

            base.OnLoaded(sender, e);
        }

        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= OnCoreWindowKeyDown;

            base.OnUnloaded(sender, e);
        }

        private void OnCoreWindowKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (VirtualKey.Enter == args.VirtualKey && IsFocused)
            {
                var text = input.Text;

                var item = new Span();

                item.Inlines.Add(new Run
                {
                    Text = text
                });

                history.Inlines.Add(item);
                input.Text = String.Empty;

                args.Handled = true;
            }
        }
    }
}
