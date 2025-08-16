using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Controls
{
    public class MoonPhaseView : SKCanvasView
    {
        public static readonly BindableProperty PhaseProperty = BindableProperty.Create(
            nameof(Phase), typeof(double), typeof(MoonPhaseView), 0.0,
            propertyChanged: (bindable, oldValue, newValue) => ((MoonPhaseView)bindable).InvalidateSurface());

        public static readonly BindableProperty IlluminationProperty = BindableProperty.Create(
            nameof(Illumination), typeof(double), typeof(MoonPhaseView), 0.0,
            propertyChanged: (bindable, oldValue, newValue) => ((MoonPhaseView)bindable).InvalidateSurface());

        public static readonly BindableProperty MoonColorProperty = BindableProperty.Create(
            nameof(MoonColor), typeof(Color), typeof(MoonPhaseView), Colors.LightGray,
            propertyChanged: (bindable, oldValue, newValue) => ((MoonPhaseView)bindable).InvalidateSurface());

        public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
            nameof(ShadowColor), typeof(Color), typeof(MoonPhaseView), Colors.DarkGray,
            propertyChanged: (bindable, oldValue, newValue) => ((MoonPhaseView)bindable).InvalidateSurface());

        public double Phase
        {
            get => (double)GetValue(PhaseProperty);
            set => SetValue(PhaseProperty, value);
        }

        public double Illumination
        {
            get => (double)GetValue(IlluminationProperty);
            set => SetValue(IlluminationProperty, value);
        }

        public Color MoonColor
        {
            get => (Color)GetValue(MoonColorProperty);
            set => SetValue(MoonColorProperty, value);
        }

        public Color ShadowColor
        {
            get => (Color)GetValue(ShadowColorProperty);
            set => SetValue(ShadowColorProperty, value);
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            var info = e.Info;
            var size = Math.Min(info.Width, info.Height);
            var center = new SKPoint(info.Width / 2, info.Height / 2);
            var radius = size / 2 - 10;

            // Ay dairesi çiz
            var moonPaint = new SKPaint
            {
                Color = MoonColor.ToSKColor(),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            canvas.DrawCircle(center, radius, moonPaint);

            // Gölge kýsmýný çiz (ay fazýna göre)
            if (Illumination < 1.0)
            {
                var shadowPaint = new SKPaint
                {
                    Color = ShadowColor.ToSKColor(),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };

                // Fazý hesapla (0-1 arasý)
                var phaseOffset = (float)(1.0 - Illumination) * 2 - 1; // -1 ile 1 arasý

                var shadowPath = new SKPath();
                var rect = new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius);
                
                shadowPath.AddArc(rect, -90, 180);
                
                if (phaseOffset < 0)
                {
                    // Sol yarým (Waning)
                    var ellipseWidth = radius * 2 * Math.Abs(phaseOffset);
                    var ellipseRect = new SKRect(center.X - (float)ellipseWidth / 2, center.Y - radius, 
                                               center.X + (float)ellipseWidth / 2, center.Y + radius);
                    shadowPath.AddOval(ellipseRect);
                }
                else
                {
                    // Sað yarým (Waxing)
                    var ellipseWidth = radius * 2 * phaseOffset;
                    var ellipseRect = new SKRect(center.X - (float)ellipseWidth / 2, center.Y - radius, 
                                               center.X + (float)ellipseWidth / 2, center.Y + radius);
                    shadowPath.AddOval(ellipseRect);
                }

                canvas.DrawPath(shadowPath, shadowPaint);
            }

            // Çerçeve çiz
            var borderPaint = new SKPaint
            {
                Color = SKColors.Gray,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };

            canvas.DrawCircle(center, radius, borderPaint);

            // Aydýnlanma yüzdesi metni
            var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                TextSize = 24,
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
            };

            var percentText = $"{Illumination * 100:F1}%";
            canvas.DrawText(percentText, center.X, center.Y + radius + 30, textPaint);
        }
    }
}