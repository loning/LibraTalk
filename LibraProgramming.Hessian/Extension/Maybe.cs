using System;

namespace LibraProgramming.Hessian.Extension
{
    internal static class Maybe
    {
        public static TResult With<TInput, TResult>(this TInput value, Func<TInput, TResult> eval)
            where TResult : class
        {
            return (null == value) ? null : eval(value);
        }

        public static TResult Return<TInput, TResult>(this TInput value, Func<TInput, TResult> eval, TResult failure)
            where TResult : class
        {
            return (null == value) ? failure : eval(value);
        }

        public static TInput If<TInput>(this TInput value, Predicate<TInput> condition)
            where TInput : class
        {
            if (null == value)
            {
                return null;
            }

            return condition(value) ? value : null;
        }

        public static TInput Unless<TInput>(this TInput value, Predicate<TInput> condition)
            where TInput : class
        {
            if (null == value)
            {
                return null;
            }

            return condition(value) ? null : value;
        }
    }
}
