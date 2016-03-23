using System;

namespace LibraProgramming.Windows.Locator
{
    /// <summary>
    /// The ServiceTypeReference class.
    /// </summary>
    public class ServiceTypeReference : IEquatable<ServiceTypeReference>
    {
        public Type Type
        {
            get;
            private set;
        }

        public string Key
        {
            get;
            private set;
        }

        public ServiceTypeReference(Type type, string key = null)
        {
            Type = type;
            Key = key;
        }

        public bool Equals(ServiceTypeReference other)
        {
            if (null == other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Type == other.Type && String.Equals(Key, other.Key);
        }
    }
}