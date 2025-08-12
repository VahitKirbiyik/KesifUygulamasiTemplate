public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateToAsync<TViewModel>(IDictionary<string, object>? parameters = null) 
        where TViewModel : BaseViewModel;
    Task GoBackAsync();
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }
    
    public async Task NavigateToAsync<TViewModel>(IDictionary<string, object>? parameters = null) 
        where TViewModel : BaseViewModel
    {
        var viewModelType = typeof(TViewModel);
        var viewModelName = viewModelType.Name.Replace("ViewModel", "Page");
        var route = $"//{viewModelName}";
        
        if (parameters != null)
        {
            var viewModel = _serviceProvider.GetService<TViewModel>();
            if (viewModel != null)
            {
                foreach (var param in parameters)
                {
                    if (viewModelType.GetProperty(param.Key) is PropertyInfo prop)
                    {
                        prop.SetValue(viewModel, param.Value);
                    }
                }
            }
        }
        
        await Shell.Current.GoToAsync(route);
    }
    
    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
