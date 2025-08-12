using System;
using Microsoft.Maui.Storage;

namespace KesifUygulamasiTemplate.Services
{
    public class SettingsService
    {
        public T GetSetting<T>(string key, T defaultValue)
        {
            try
            {
                if (Preferences.Default.ContainsKey(key))
                {
                    if (typeof(T) == typeof(string))
                    {
                        return (T)(object)Preferences.Default.Get(key, (string)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        return (T)(object)Preferences.Default.Get(key, (int)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        return (T)(object)Preferences.Default.Get(key, (bool)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        return (T)(object)Preferences.Default.Get(key, (double)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        return (T)(object)Preferences.Default.Get(key, (float)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        return (T)(object)Preferences.Default.Get(key, (long)(object)defaultValue);
                    }
                    else if (typeof(T) == typeof(DateTime))
                    {
                        var ticks = Preferences.Default.Get(key, ((DateTime)(object)defaultValue).Ticks);
                        return (T)(object)new DateTime(ticks);
                    }
                }
                
                return defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public void SaveSetting<T>(string key, T value)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    Preferences.Default.Set(key, (string)(object)value);
                }
                else if (typeof(T) == typeof(int))
                {
                    Preferences.Default.Set(key, (int)(object)value);
                }
                else if (typeof(T) == typeof(bool))
                {
                    Preferences.Default.Set(key, (bool)(object)value);
                }
                else if (typeof(T) == typeof(double))
                {
                    Preferences.Default.Set(key, (double)(object)value);
                }
                else if (typeof(T) == typeof(float))
                {
                    Preferences.Default.Set(key, (float)(object)value);
                }
                else if (typeof(T) == typeof(long))
                {
                    Preferences.Default.Set(key, (long)(object)value);
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    Preferences.Default.Set(key, ((DateTime)(object)value).Ticks);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving setting: {ex.Message}");
            }
        }

        public void RemoveSetting(string key)
        {
            try
            {
                if (Preferences.Default.ContainsKey(key))
                {
                    Preferences.Default.Remove(key);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing setting: {ex.Message}");
            }
        }

        public void ClearAllSettings()
        {
            try
            {
                Preferences.Default.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing settings: {ex.Message}");
            }
        }
    }
}
