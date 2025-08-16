using Android.Appwidget;
using Android.App;
using Android.Content;
using Android.Widget;

namespace KesifUygulamasiTemplate.Platforms.Android.Widgets
{
    [BroadcastReceiver(Label = "Ay Pusulasý Widget", Exported = true)]
    [IntentFilter(new[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/moonwidgetprovider")]
    public class MoonWidgetProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            foreach (var widgetId in appWidgetIds)
            {
                var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.moonwidget);
                // Verileri doldur
                remoteViews.SetTextViewText(Resource.Id.txtPhase, "Dolunay");
                remoteViews.SetTextViewText(Resource.Id.txtRise, "20:15");
                remoteViews.SetTextViewText(Resource.Id.txtSet, "06:10");
                // Týklama ile uygulama açma
                var intent = new Intent(context, typeof(MainActivity));
                var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);
                remoteViews.SetOnClickPendingIntent(Resource.Id.widgetRoot, pendingIntent);
                appWidgetManager.UpdateAppWidget(widgetId, remoteViews);
            }
        }
    }
}