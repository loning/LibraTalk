using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using LibraProgramming.Windows.UI.Xaml.Primitives;
using LibraProgramming.Windows.UI.Xaml.Primitives.Collections;
using LibraProgramming.Windows.UI.Xaml.Primitives.Commands;

namespace LibraProgramming.Windows.UI.Xaml
{
    public class DialogCommandExecutingEventArgs : EventArgs
    {
        public IDialogCommand Command
        {
            get;
        }

        public bool Cancel
        {
            get;
            set;
        }

        public DialogCommandExecutingEventArgs(IDialogCommand command)
        {
            Command = command;
        }
    }

    public class DialogCommandExecuteEventArgs : EventArgs
    {
        public IDialogCommand Command
        {
            get;
        }

        public DialogCommandExecuteEventArgs(IDialogCommand command)
        {
            Command = command;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Type = typeof(Popup), Name = RootPopupPartName)]
    [TemplatePart(Type = typeof(Grid), Name = RootOverlayPartName)]
    [TemplatePart(Type = typeof(Border), Name = RootDialogPartName)]
    [TemplatePart(Type = typeof(Grid), Name = ButtonsPanelContainerPartName)]
    [TemplatePart(Type = typeof(ItemsControl), Name = ButtonsHostPartName)]
    [ContentProperty(Name = "Content")]
    public sealed class CustomDialog : ContentControlPrimitive
    {
        private const string RootPopupPartName = "PART_RootPopup";
        private const string RootOverlayPartName = "PART_RootOverlay";
        private const string RootDialogPartName = "PART_RootDialog";
        private const string ButtonsPanelContainerPartName = "PART_ButtonsGrid";
        private const string ButtonsHostPartName = "PART_ButtonsHost";

        public static readonly DependencyProperty DialogWidthProperty;
        public static readonly DependencyProperty DialogHorizontalAlignmentProperty;
        public static readonly DependencyProperty DialogMarginProperty;
        public static readonly DependencyProperty IsOpenProperty;
        public static readonly DependencyProperty MinDialogWidthProperty;
        public static readonly DependencyProperty MaxDialogWidthProperty;
        public static readonly DependencyProperty OverlayProperty;
        public static readonly DependencyProperty TitleProperty;
        public static readonly DependencyProperty TitleTemplateProperty;
        public static readonly DependencyProperty CommandsPanelProperty;
        public static readonly DependencyProperty CommandTemplateProperty;

        private Popup popup;
        private Grid overlayGrid;
        private Border dialogContainer;
        private Grid buttonsGrid;
        private ItemsControl buttonsHost;
        //        private DialogCommandsPanelTemplate commandsPanel;
        private DialogCommandDispatcher commandDispatcher;

        /// <summary>
        /// 
        /// </summary>
        public DialogCommandCollection Commands
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate CommandTemplate
        {
            get
            {
                return (DataTemplate)GetValue(CommandTemplateProperty);
            }
            set
            {
                SetValue(CommandTemplateProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
/*
        public DialogCommandsPanelTemplate CommandsPanel
        {
            get
            {
                return (DialogCommandsPanelTemplate) GetValue(CommandsPanelProperty);
            }
            set
            {
                SetValue(CommandsPanelProperty, value);
            }
        }
*/
        public ItemsPanelTemplate CommandsPanel
        {
            get
            {
                return (ItemsPanelTemplate)GetValue(CommandsPanelProperty);
            }
            set
            {
                SetValue(CommandsPanelProperty, value);
            }
        }

        public double MinDialogWidth
        {
            get
            {
                return (System.Double)GetValue(MinDialogWidthProperty);
            }
            set
            {
                SetValue(MinDialogWidthProperty, value);
            }
        }

        public double MaxDialogWidth
        {
            get
            {
                return (System.Double)GetValue(MaxDialogWidthProperty);
            }
            set
            {
                SetValue(MaxDialogWidthProperty, value);
            }
        }

        public double DialogWidth
        {
            get
            {
                return (System.Double)GetValue(DialogWidthProperty);
            }
            set
            {
                SetValue(DialogWidthProperty, value);
            }
        }

        public HorizontalAlignment DialogHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(DialogHorizontalAlignmentProperty);
            }
            set
            {
                SetValue(DialogHorizontalAlignmentProperty, value);
            }
        }

        public Thickness DialogMargin
        {
            get
            {
                return (Thickness)GetValue(DialogMarginProperty);
            }
            set
            {
                SetValue(DialogMarginProperty, value);
            }
        }

        public object Title
        {
            get
            {
                return GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TitleTemplateProperty);
            }
            set
            {
                SetValue(TitleTemplateProperty, value);
            }
        }

        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public Brush Overlay
        {
            get
            {
                return (Brush)GetValue(OverlayProperty);
            }
            set
            {
                SetValue(OverlayProperty, value);
            }
        }

        public event EventHandler Opened;

        public event EventHandler Closed;

        public event EventHandler<DialogCommandExecutingEventArgs> ExecutingCommand;

        public event EventHandler<DialogCommandExecuteEventArgs> ExecuteCommand;

        public CustomDialog()
        {
            DefaultStyleKey = typeof(CustomDialog);
            commandDispatcher = new DialogCommandDispatcher(this);
            Commands = new DialogCommandCollection(commandDispatcher);

            Loaded += OnControlLoaded;
            Unloaded += OnControlUnloaded;

//            Commands.CollectionChanged += OnCommandsCollectionChanged;
        }

        static CustomDialog()
        {
            CommandsPanelProperty = DependencyProperty
                .Register(
                    "CommandsPanel",
                    //                    typeof (DialogCommandsPanelTemplate),
                    typeof(ItemsPanelTemplate),
                    typeof(CustomDialog),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnCommandsPanelPropertyChanged)
                );
            CommandTemplateProperty = DependencyProperty
                .Register(
                    "CommandTemplate",
                    typeof(DataTemplate),
                    typeof(CustomDialog),
                    new PropertyMetadata(null, OnCommandTemplatePropertyChanged)
                );
            MinDialogWidthProperty = DependencyProperty
                .Register(
                    "MinDialogWidth",
                    typeof(System.Double),
                    typeof(CustomDialog),
                    new PropertyMetadata(0.0d)
                );
            MaxDialogWidthProperty = DependencyProperty
                .Register(
                    "MaxDialogWidth",
                    typeof(System.Double),
                    typeof(CustomDialog),
                    new PropertyMetadata(Double.PositiveInfinity)
                );
            DialogWidthProperty = DependencyProperty
                .Register(
                    "DialogWidth",
                    typeof(System.Double),
                    typeof(CustomDialog),
                    new PropertyMetadata(System.Double.NaN)
                );
            DialogHorizontalAlignmentProperty = DependencyProperty
                .Register(
                    "DialogHorizontalAlignment",
                    typeof(HorizontalAlignment),
                    typeof(CustomDialog),
                    new PropertyMetadata(HorizontalAlignment.Center, OnDialogHorizontalAlignmentPropertyChanged)
                );
            DialogMarginProperty = DependencyProperty
                .Register(
                    "DialogMargin",
                    typeof(Thickness),
                    typeof(CustomDialog),
                    new PropertyMetadata(default(Thickness))
                );
            TitleProperty = DependencyProperty
                .Register(
                    "Title",
                    typeof(object),
                    typeof(CustomDialog),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            TitleTemplateProperty = DependencyProperty
                .Register(
                    "TitleTemplate",
                    typeof(DataTemplate),
                    typeof(CustomDialog),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            OverlayProperty = DependencyProperty
                .Register(
                    "Overlay",
                    typeof(Brush),
                    typeof(CustomDialog),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            IsOpenProperty = DependencyProperty
                .Register(
                    "IsOpen",
                    typeof(bool),
                    typeof(CustomDialog),
                    new PropertyMetadata(false, OnIsOpenPropertyChanged)
                );
        }

        public void Close()
        {
            if (IsOpen)
            {
                IsOpen = false;
            }
        }

        protected override void OnApplyTemplate()
        {
            if (null != popup)
            {
                popup.Opened -= OnPopupOpened;
                popup.Closed -= OnPopupClosed;
            }

            popup = GetTemplatePart<Popup>(RootPopupPartName);

            popup.HorizontalOffset = 0.0d;
            popup.VerticalOffset = 0.0d;
            popup.Opened += OnPopupOpened;
            popup.Closed += OnPopupClosed;

            overlayGrid = GetTemplatePart<Grid>(RootOverlayPartName);
            dialogContainer = GetTemplatePart<Border>(RootDialogPartName);
            buttonsGrid = GetTemplatePart<Grid>(ButtonsPanelContainerPartName);
            buttonsHost = GetTemplatePart<ItemsControl>(ButtonsHostPartName);

            base.OnApplyTemplate();

            ResizeContainers();

            buttonsHost.DataContext = Commands;
//            buttonsHost.Items.VectorChanged += OnButtonsHostVectorChanged;

//            OnCommandsPanelChanged(CommandsPanel);
            OnCommandsCollectionChanged(Commands, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void DoInsertCommands(int index, ICollection items)
        {
            /*foreach (var item in items)
            {
                var container1 = buttonsHost.ContainerFromIndex(index++) as FrameworkElement;
                var container2 = buttonsHost.ContainerFromItem(item) as FrameworkElement;

                container1.Tapped += OnContainerTapped;
            }*/

            /*var commands = commandsPanel.Template.Children;

            foreach (var item in items)
            {
                var container = CommandTemplate.LoadContent() as FrameworkElement;

                if (null != container)
                {
//                    var command = (DialogCommandBase) item;

                    container.DataContext = item;
                    container.Tapped += OnCommandTapped;

                    commands.Insert(index++, container);
                }
            }*/
        }

        private void OnCommandsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    DoInsertCommands(e.NewStartingIndex, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    //                    commandsPanel.Template.Children.Clear();
                    DoInsertCommands(0, Commands);
                    break;
            }
            /*

                        commandsPanel.Template.Children.Add(new Button
                        {
                            Content = "TEST1"
                        });
            */
        }

        private void OnPopupOpened(object sender, object e)
        {
            DoPopupOpened(EventArgs.Empty);
        }

        private void OnPopupClosed(object sender, object e)
        {
            DoPopupClosed(EventArgs.Empty);
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += OnWindowSizeChanged;
        }

        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            ResizeContainers();
        }

        private void ResizeContainers()
        {
            var bounds = Window.Current.Bounds;

            popup.Width = bounds.Width;
            popup.Height = bounds.Height;
            overlayGrid.Width = bounds.Width;
            overlayGrid.Height = bounds.Height;
        }

        /*
                private void ArrangeDialog()
                {
                    dialogContainer.HorizontalAlignment = HorizontalAlignment.Left;
                    dialogContainer.VerticalAlignment = VerticalAlignment.Top;
                    dialogContainer.Margin = new Thickness(10, 10, 0, 0);
                }
        */

        private void DoPopupOpened(EventArgs args)
        {
            var handler = Opened;
            handler?.Invoke(this, args);
        }

        private void DoPopupClosed(EventArgs args)
        {
            var handler = Closed;
            handler?.Invoke(this, args);
        }

        private void OnIsOpenChanged(bool value)
        {
            if (value)
            {
                ApplyTemplate();
            }
        }

        private void OnCommandTapped(object sender, TappedRoutedEventArgs e)
        {
            var command = ((FrameworkElement)sender).DataContext as IDialogCommand;

            if (null != command)
            {
                commandDispatcher.ExecuteCommand(command);
                ExecuteCommand?.Invoke(this, new DialogCommandExecuteEventArgs(command));
            }
        }

        private void OnDialogHorizontalAlignmentChanged()
        {
        }

        private void OnCommandsPanelChanged(ItemsPanelTemplate current)
        {
            if (!IsTemplateApplied)
            {
                return;
            }


            /*if (null != commandsPanel)
            {
                var panel = commandsPanel.Template;

                buttonsGrid.Children.Remove(panel);
                panel.ClearValue(Grid.ColumnProperty);
            }

            commandsPanel = current;

            if (null != commandsPanel)
            {
                var panel = commandsPanel.Template;

                Grid.SetColumn(panel, 1);
                buttonsGrid.Children.Add(panel);
//                panel.HorizontalAlignment = HorizontalAlignment.Right;
//                panel.VerticalAlignment = VerticalAlignment.Stretch;
            }*/
        }

        private void OnCommandTemplateChanged()
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            ;
        }

/*
        private void OnButtonsHostVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
        {
            switch (e.CollectionChange)
            {
                case CollectionChange.ItemChanged:
                {
                    var container = buttonsHost.ContainerFromIndex((int)e.Index) as FrameworkElement;

                    container.Tapped += OnContainerTapped;

                    break;
                }

                case CollectionChange.ItemInserted:
                case CollectionChange.ItemRemoved:
                case CollectionChange.Reset:
                    break;
            }
        }
*/

        private void OnContainerTapped(object sender, TappedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnIsOpenPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CustomDialog)source).OnIsOpenChanged((bool)e.NewValue);
        }

        private static void OnDialogHorizontalAlignmentPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CustomDialog)source).OnDialogHorizontalAlignmentChanged();
        }

        private static void OnCommandTemplatePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CustomDialog)source).OnCommandTemplateChanged();
        }

        private static void OnCommandsPanelPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CustomDialog)source).OnCommandsPanelChanged((ItemsPanelTemplate)e.NewValue);
        }
    }
}
