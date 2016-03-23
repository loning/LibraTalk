using System;
using System.Linq.Expressions;

namespace LibraProgramming.Windows.UI.Xaml.Infrastructure
{
    internal static class ExpressionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName(this Expression expression)
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