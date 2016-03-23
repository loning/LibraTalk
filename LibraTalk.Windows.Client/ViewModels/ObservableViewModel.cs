using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using LibraTalk.Windows.Client.Properties;

namespace LibraTalk.Windows.Client.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ObservableViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TValue>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;

            DoPropertyChanged(propertyName);

            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void DoPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected void DoPropertyChanged([NotNull] Expression<Func<object>> expression)
        {
            var propertyName = GetPropertyName(expression);
            DoPropertyChanged(propertyName);
        }

        internal static string GetPropertyName(Expression expression)
        {
            while (true)
            {
                var lambda = expression as LambdaExpression;

                if (null != lambda)
                {
                    expression = lambda.Body;
                    continue;
                }

                var access = expression as MemberExpression;

                if (null != access)
                {
                    return access.Member.Name;
                }

                var unary = expression as UnaryExpression;

                if (null == unary)
                {
                    throw new NotSupportedException();
                }

                if (ExpressionType.Convert == unary.NodeType)
                {
                    expression = unary.Operand;
                    continue;
                }

                break;
            }

            throw new NotSupportedException();
        }
    }
}