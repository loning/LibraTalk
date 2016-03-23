using System;

namespace LibraProgramming.Windows.Locator
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ServiceAttribute : Attribute
    {
        public string Key
        {
            get;
            set;
        }
    }
}