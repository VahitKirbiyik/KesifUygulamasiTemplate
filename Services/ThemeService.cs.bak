using System;
using Microsoft.Maui.Storage;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public class ThemeService
    {
        private const Models.AppTheme DefaultTheme = Models.AppTheme.Light;
        private const string ThemeKey = "AppTheme";

        public Models.AppTheme CurrentTheme { get; private set; } = DefaultTheme;

        public ThemeService()
        {
            LoadTheme();
        }

        public void SetTheme(Models.AppTheme theme)
        {
            CurrentTheme = theme;
            Preferences.Set(ThemeKey, theme.ToString());
            ApplyTheme(theme);
        }

        public void ApplyTheme(Models.AppTheme theme)
        {
            Microsoft.Maui.Controls.Application.Current.UserAppTheme = theme switch
            {
                Models.AppTheme.Light => Microsoft.Maui.ApplicationModel.AppTheme.Light,
                Models.AppTheme.Dark => Microsoft.Maui.ApplicationModel.AppTheme.Dark,
                Models.AppTheme.Custom => Microsoft.Maui.ApplicationModel.AppTheme.Light, // Custom için Light baz alınır
                _ => Microsoft.Maui.ApplicationModel.AppTheme.Light
            };
        }

        public void LoadTheme()
        {
            var themeStr = Preferences.Get(ThemeKey, DefaultTheme.ToString());
            if (Enum.TryParse<Models.AppTheme>(themeStr, out var theme))
                CurrentTheme = theme;
            else
                CurrentTheme = DefaultTheme;
            ApplyTheme(CurrentTheme);
        }
    }
}
