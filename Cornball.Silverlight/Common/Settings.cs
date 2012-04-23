using System;
using System.IO.IsolatedStorage;

namespace Cornball.Common
{
    internal static class Settings
    {
        private static bool? _soundEnabled;

        internal static bool SoundEnabled
        {
            get
            {
                if (!_soundEnabled.HasValue)
                {
                    _soundEnabled = Convert.ToBoolean(Get("SoundEnabled") ?? true);
                }
                return _soundEnabled.Value;
            }
            set
            {
                _soundEnabled = value;
                Set("SoundEnabled", value);
            }
        }

        private static object Get(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                return IsolatedStorageSettings.ApplicationSettings[key];
            }
            return null;
        }

        private static void Set(string key, object value)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);
            }
        }
    }
}