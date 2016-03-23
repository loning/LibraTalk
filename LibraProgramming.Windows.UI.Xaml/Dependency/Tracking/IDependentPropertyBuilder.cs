using System;

namespace LibraProgramming.Windows.UI.Xaml.Dependency.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IDependentPropertyBuilder<TModel> : IDependencyPropertySelector<TModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculator"></param>
        /// <returns></returns>
        IDependencyPropertySelector<TModel> CalculatedBy(Func<TModel, object> calculator);
    }
}