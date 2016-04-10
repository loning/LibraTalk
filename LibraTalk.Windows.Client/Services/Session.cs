using System;
using Windows.Storage;

namespace LibraTalk.Windows.Client.Services
{
    public static class Session
    {
        public static Guid GetId()
        {
            const string key = "Session.Id";

            Guid id;

            var value = ApplicationData.Current.LocalSettings.Values[key];

            if (null == value)
            {
                id = Guid.NewGuid();
                ApplicationData.Current.LocalSettings.Values[key] = id;
            }
            else
            {
                id = (Guid)value;
            }

            return id;
        }
    }
}