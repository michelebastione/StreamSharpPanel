namespace StreamSharpPanel.Services;

public class StateService
{
    internal Action? OnDarkModeChanged { get; set; }
    internal bool IsDarkMode 
    { 
        get; 
        set
        {
            field = value;
            OnDarkModeChanged?.Invoke();
        }
    } = true;

    internal Action? OnPageLoadingChanged { get; set; }
    internal bool IsPageLoading
    { 
        get; 
        set
        {
            field = value;
            OnPageLoadingChanged?.Invoke();
        }
    }
}
