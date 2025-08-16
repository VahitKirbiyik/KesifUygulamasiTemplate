public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("compass", typeof(Views.CompassPage));
        Routing.RegisterRoute("calibration", typeof(Views.CalibrationPage));
    }
}
