// NavigationService'in mesajlaþma sistemiyle entegrasyonu
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessenger _messenger;
    
    public NavigationService(IServiceProvider serviceProvider, IMessenger messenger)
    {
        _serviceProvider = serviceProvider;
        _messenger = messenger;
    }
    
    public async Task NavigateToAsync<TViewModel>(IDictionary<string, object>? parameters = null) 
        where TViewModel : BaseViewModel
    {
        // ...mevcut kod...
        
        // Navigasyon öncesi mesaj gönder
        _messenger.Send(new NavigationMessage(typeof(TViewModel), NavigationDirection.Forward));
        
        await Shell.Current.GoToAsync(route);
        
        // Navigasyon sonrasý mesaj gönder
        _messenger.Send(new NavigationCompletedMessage(typeof(TViewModel)));
    }
}

// NavigationMessage.cs
public record NavigationMessage(Type TargetViewModelType, NavigationDirection Direction);
public record NavigationCompletedMessage(Type TargetViewModelType);
public enum NavigationDirection { Forward, Backward }
