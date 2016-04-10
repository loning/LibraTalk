using System;

namespace LibraProgramming.Hessian
{
    public partial class HessianObjectSerializerResolver
    {
        private static IObjectSerializerResolver current;

        public static IObjectSerializerResolver Current
        {
            get
            {
                return current ?? (current = new HessianObjectSerializerResolver());
            }
            private set
            {
                current = value;
            }
        }

        public static void SetCurrent(IObjectSerializerResolver value)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Current = value;
        }
    }
}