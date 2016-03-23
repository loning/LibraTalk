using System;
using System.Linq.Expressions;
using LibraProgramming.Windows.UI.Xaml.Core;

namespace LibraProgramming.Windows.UI.Xaml.Dependency.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IDependencyTrackerBuilder<TModel> : IObjectBuilder<IDependencyTracker<TModel>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IDependentPropertyBuilder<TModel> RaiseProperty(Expression<Func<TModel, object>> expression);
    }
}