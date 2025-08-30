using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;

namespace KesifUygulamasiTemplate.Services
{
    public class AppCenterAnalyticsService
    {
        private bool _isInitialized = false;

        public AppCenterAnalyticsService()
        {
            InitializeAppCenter();
        }

        private void InitializeAppCenter()
        {
            if (_isInitialized) return;

            // AppCenter portalından alınan anahtarlarınızı buraya ekleyin
            string androidKey = "android-appcenter-key";
            string iosKey = "ios-appcenter-key";

            AppCenter.Start($"android={androidKey};ios={iosKey}", typeof(Analytics), typeof(Crashes));
            _isInitialized = true;
        }

        public void TrackEvent(string eventName, Dictionary<string, string> properties = null)
        {
            Analytics.TrackEvent(eventName, properties);
        }

        public void TrackError(Exception exception, Dictionary<string, string> properties = null)
        {
            Crashes.TrackError(exception, properties);
        }
    }
}
