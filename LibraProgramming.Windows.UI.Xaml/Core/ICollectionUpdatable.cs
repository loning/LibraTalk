using System;

namespace LibraProgramming.Windows.UI.Xaml.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICollectionUpdatable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void BeginUpdate();

        /// <summary>
        /// 
        /// </summary>
        void EndUpdate();

        /// <summary>
        /// 
        /// </summary>
        void Reset();
    }
}