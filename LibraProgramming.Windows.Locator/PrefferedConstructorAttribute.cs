using System;

namespace LibraProgramming.Windows.Locator
{
    /// <summary>
    /// Marks preffered ctor for <see cref="ServiceLocator" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class PrefferedConstructorAttribute : Attribute
    {
    }
}