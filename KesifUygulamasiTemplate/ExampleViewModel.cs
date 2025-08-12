public class ExampleViewModel : BaseViewModel
{
    private string _title;
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}
