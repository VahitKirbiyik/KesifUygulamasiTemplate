using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Behaviors
{
    public class RotationBehavior : Behavior<VisualElement>
    {
        public static readonly BindableProperty IsRotatingProperty =
            BindableProperty.Create(nameof(IsRotating), typeof(bool), typeof(RotationBehavior), false,
                propertyChanged: OnIsRotatingChanged);

        public static readonly BindableProperty DurationProperty =
            BindableProperty.Create(nameof(Duration), typeof(uint), typeof(RotationBehavior), (uint)1000);

        public bool IsRotating
        {
            get => (bool)GetValue(IsRotatingProperty);
            set => SetValue(IsRotatingProperty, value);
        }

        public uint Duration
        {
            get => (uint)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        private VisualElement? _associatedObject;

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            _associatedObject = bindable;

            if (IsRotating)
            {
                StartRotationAnimation();
            }
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            _associatedObject = null;
            base.OnDetachingFrom(bindable);
        }

        private static void OnIsRotatingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (RotationBehavior)bindable;

            if (behavior._associatedObject == null)
                return;

            if ((bool)newValue)
            {
                behavior.StartRotationAnimation();
            }
            else
            {
                behavior.StopRotationAnimation();
            }
        }

        private void StartRotationAnimation()
        {
            if (_associatedObject == null) return;

            // Döndürme animasyonunu başlat
            Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(_associatedObject);

            _associatedObject.Animate(
                "Rotation",
                animation: new Animation(v => _associatedObject.Rotation = v, 0, 360),
                length: Duration,
                easing: Easing.Linear,
                repeat: () => IsRotating
            );
        }

        private void StopRotationAnimation()
        {
            if (_associatedObject == null) return;
            Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(_associatedObject);
        }
    }
}
