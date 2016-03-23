using System;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MissingTemplatePartException : Exception
    {
        private readonly Type partType;

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>
        /// The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        public override string Message => ("Part name: " + base.Message + Environment.NewLine + "Part type: " + partType.FullName);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class.
        /// </summary>
        public MissingTemplatePartException(Type partType, string name)
            : base(name)
        {
            this.partType = partType;
        }
    }
}