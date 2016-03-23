using System;

namespace LibraProgramming.Windows.UI.Xaml.Dependency.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IDependencyTracker<in TModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IDisposable Subscribe(TModel target);
    }
}