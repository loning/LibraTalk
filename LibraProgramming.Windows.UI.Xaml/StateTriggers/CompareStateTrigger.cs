using System;
using System.Globalization;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
    public enum Comparison
    {
        NotComparable,
        Equal,
        LessThan,
        GreaterThan
    }

    public class CompareStateTrigger : CustomStateTrigger
    {
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty ExpectedProperty;
        public static readonly DependencyProperty ComparisonProperty;

        public Comparison Comparison
        {
            get
            {
                return (Comparison) GetValue(ComparisonProperty);
            }
            set
            {
                SetValue(ComparisonProperty, value);
            }
        }

        public object Expected
        {
            get
            {
                return GetValue(ExpectedProperty);
            }
            set
            {
                SetValue(ExpectedProperty, value);
            }
        }

        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        static CompareStateTrigger()
        {
            ValueProperty = DependencyProperty
                .Register(
                    "Value",
                    typeof (object),
                    typeof (CompareStateTrigger),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnValuePropertyChanged)
                );
            ExpectedProperty = DependencyProperty
                .Register(
                    "Expected",
                    typeof (object),
                    typeof (CompareStateTrigger),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnExpectedPropertyChanged)
                );
            ComparisonProperty = DependencyProperty
                .Register(
                    "Comparison",
                    typeof (Comparison),
                    typeof (CompareStateTrigger),
                    new PropertyMetadata(Comparison.Equal, OnComparisonPropertyChanged)
                );
        }

        private void UpdateTrigger()
        {
            IsActive = Comparison == EvaluateComparison();
        }

        private Comparison EvaluateComparison()
        {
            var op1 = Value;
            var op2 = Expected;

            if (op1.Equals(op2))
            {
                if (Comparison.Equal == Comparison)
                {
                    return Comparison.Equal;
                }
            }

            if (null != op1 && null != op2)
            {
                if (op1.GetType() != op2.GetType())
                {
                    if (op1 is Enum)
                    {
                        op2 = Enum.Parse(op1.GetType(), op2.ToString());
                    }
                    else if (op2 is Enum)
                    {
                        op1 = Enum.Parse(op2.GetType(), op1.ToString());
                    }
                    else if(op1 is IComparable)
                    {
                        op2 = Convert.ChangeType(op2, op1.GetType(), CultureInfo.InvariantCulture);
                    }
                    else if(op2 is IComparable)
                    {
                        op1 = Convert.ChangeType(op1, op2.GetType(), CultureInfo.InvariantCulture);
                    }
                }

                if (op1.GetType() == op2.GetType())
                {
                    var comp = op1 as IComparable;

                    if (null == comp)
                    {
                        return Comparison.NotComparable;
                    }

                    var result = comp.CompareTo(op2);

                    if (0 == result)
                    {
                        return Comparison.Equal;
                    }

                    return 0 > result ? Comparison.LessThan : Comparison.GreaterThan;
                }
            }

            return Comparison.NotComparable;
        }

        private static void OnComparisonPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CompareStateTrigger)source).UpdateTrigger();
        }

        private static void OnExpectedPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CompareStateTrigger)source).UpdateTrigger();
        }

        private static void OnValuePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CompareStateTrigger)source).UpdateTrigger();
        }
    }
}