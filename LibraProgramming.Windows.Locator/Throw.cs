﻿using System;

namespace LibraProgramming.Windows.Locator
{
    internal static class Throw
    {
        public static void ArgumentNull(string paramName)
        {
            throw new ArgumentNullException(paramName, "");
        }

        public static void MissingServiceRegistration(Type serviceType, string paramName)
        {
            throw new ArgumentException(paramName, "");
        }

        public static void UnsupportedServiceType(Type serviceType)
        {
            var message = String.Format("Unsupported type:{0}", serviceType.Name);
            throw new ArgumentException(message);
        }

        public static void CyclicServiceReference(Type type, string argumentName, Type serviceType, string key)
        {
            var message = String.IsNullOrEmpty(key)
                ? String.Format("Service: {0} has cyclic reference", serviceType)
                : String.Format("Service: {0} with key: {1} has cyclic reference", serviceType, key);

            throw new ServiceLocatorException(message);
        }
    }
}